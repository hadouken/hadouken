require "rubygems"
require "bundler"
Bundler.setup
$: << './'

require 'albacore'
require 'semver'
require 'rake/clean'

require 'tools/buildscripts/environment'
require 'tools/buildscripts/utils'

CLOBBER.include("build/*")

task :default => [ :alpha ]

task :alpha => [ :clobber, "env:alpha", "env:release", :build_x86, :reset, :build_x64 ]
task :beta => [ :clobber, "env:beta", "env:release", :build_x86, :reset, :build_x64 ]
task :rc => [ :clobber, "env:rc", "env:release", :build_x86, :reset, :build_x64 ]
task :ga => [ :clobber, "env:ga", "env:release", :build_x86, :reset, :build_x64 ]

task :build_x86 => [ "arch:x86", :version, :build, :test, :output, :zip_webui, :zip, :msi ]
task :build_x64 => [ "arch:x64", :version, :build, :test, :output, :zip_webui, :zip, :msi ]

task :reset do
    Rake::Task["version"].reenable
    Rake::Task["build"].reenable
    Rake::Task["test"].reenable
    Rake::Task["test_nunit"].reenable
    Rake::Task["test_teamcity"].reenable
    Rake::Task["output"].reenable
    Rake::Task["zip_webui"].reenable
    Rake::Task["zip"].reenable
    Rake::Task["msi"].reenable
end

desc "Build"
msbuild :build => :version do |msb|
    puts "##teamcity[progressMessage 'Compiling code']"
    
    msb.properties :configuration => "Release"
    msb.properties :platform      => BUILD_PLATFORM
    msb.targets :Clean, :Build
    
    msb.solution = "Hadouken.sln"
end

desc "Versioning"
assemblyinfo :version => "env:common" do |asm|    
    asm.version = BUILD_VERSION
    asm.file_version = BUILD_VERSION
    
    asm.company_name = "Hadouken"
    asm.product_name = "Hadouken"
    asm.copyright = "2012"
    asm.namespaces = "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security", "Hadouken.Reflection"
    
    asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION} (#{BUILD_PLATFORM})", # disposed as product version in explorer
        :CLSCompliantAttribute => false,
        :AssemblyConfiguration => "#{CONFIGURATION}"
    
    asm.com_visible = false
    
    asm.output_file = "src/Shared/CommonAssemblyInfo.cs"
end

desc "Test"
task :test => :build do
    puts "##teamcity[progressMessage 'Running unit tests']"
    FileUtils.mkdir_p "build/reports" unless FileTest.exists?("build/reports")
    
    if(ENV['TEAMCITY_VERSION'])
        Rake::Task["test_teamcity"].invoke
    else
        Rake::Task["test_nunit"].invoke
    end
end

task :test_teamcity => :build do
    puts "#{ENV['NUNIT_LAUNCHER']} v4.0 #{BUILD_PLATFORM} NUnit-2.6.0 src/Tests/Hadouken.UnitTests/bin/#{BUILD_PLATFORM}/#{CONFIGURATION}/Hadouken.UnitTests.dll"
    system "#{ENV['NUNIT_LAUNCHER']} v4.0 #{BUILD_PLATFORM} NUnit-2.6.0 src/Tests/Hadouken.UnitTests/bin/#{BUILD_PLATFORM}/#{CONFIGURATION}/Hadouken.UnitTests.dll"
end

task :test_nunit => :build do
    nunitcmd = "tools/nunit-2.6.0.12051/bin/nunit-console-x86.exe"

    if BUILD_PLATFORM == "x64"
        nunitcmd = "tools/nunit-2.6.0.12051/bin/nunit-console.exe"
    end
    system "#{nunitcmd} /framework:v4.0.30319 /xml:build/reports/nunit.xml src/Tests/Hadouken.UnitTests/bin/#{BUILD_PLATFORM}/#{CONFIGURATION}/Hadouken.UnitTests.dll"
end

desc "Output"
task :output => :build do
    puts "##teamcity[progressMessage 'Outputting binaries']"
    
    copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/#{BUILD_PLATFORM}/#{CONFIGURATION}/", "*.{dll,exe}", "build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}"
    copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/#{BUILD_PLATFORM}/#{CONFIGURATION}/#{BUILD_PLATFORM}/", "*.{dll,exe}", "build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}"
    
    copy_files "src/Config/#{CONFIGURATION}/", "*.{config}", "build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}"
end

desc "Zip"
zip :zip => :output do |zip|
    zip.directories_to_zip "build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}"
    zip.output_file = "hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}.zip"
    zip.output_path = "#{File.dirname(__FILE__)}/build/#{BUILD_PLATFORM}/"
end

desc "Zip WebUI"
zip :zip_webui do |zip|
    zip.directories_to_zip "src/WebUI"
    zip.output_file = "webui.zip"
    zip.output_path = "#{File.dirname(__FILE__)}/build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}/"
end

desc "MSI"
task :msi => :output do
    system "tools/wix-3.6rc/candle.exe -ext WixFirewallExtension -ext WixUtilExtension -dPlatform=#{BUILD_PLATFORM} -dBuildVersion=#{BUILD_VERSION} -dBinDir=build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM} -out src/Installer/ src/Installer/Hadouken.wxs src/Installer/UI/ConfigDialog.wxs src/Installer/UI/ConfigInvalidDialog.wxs src/Installer/Components/Config.wxs src/Installer/Components/Core.wxs src/Installer/Components/Firewall.wxs  src/Installer/Components/Lib.wxs  src/Installer/Components/Service.wxs"
    system "tools/wix-3.6rc/light.exe -ext WixUIExtension -ext WixFirewallExtension -ext WixUtilExtension -sval -pdbout src/Installer/Hadouken.wixpdb -out build/#{BUILD_PLATFORM}/hdkn-#{BUILD_VERSION}-#{BUILD_PLATFORM}.msi src/Installer/Hadouken.wixobj src/Installer/ConfigDialog.wixobj src/Installer/ConfigInvalidDialog.wixobj src/Installer/Config.wixobj src/Installer/Core.wixobj src/Installer/Firewall.wixobj  src/Installer/Lib.wixobj  src/Installer/Service.wixobj"
end