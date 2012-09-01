require "rubygems"
require "bundler/setup"

require "albacore"
require "semver"
require "rake/clean"

def version(str)
  ver = /v?(\d+)\.(\d+)\.(\d+)\.?(\d+)?/i.match(str).to_a()
  ver[1,4].map{|s|s.to_i} unless ver == nil or ver.empty?
end

def copy_files(from_dir, file_pattern, out_dir)
  puts "copying from #{from_dir} to #{out_dir}, with pattern: #{file_pattern}"
  FileUtils.mkdir_p out_dir unless FileTest.exists?(out_dir)
  Dir.glob(File.join(from_dir, file_pattern)){|file|
    copy(file, out_dir) if File.file?(file)
  }
end

CLOBBER.include("build/*")

task :default => [ :clobber, :version, :build, :test, :output, :zip_webui, :zip, :msi ]

desc "Build"
msbuild :build => :version do |msb|
    msb.properties :configuration => "Release"
    msb.targets :Clean, :Build
    msb.solution = "Hadouken.sln"
end

desc "Versioning"
assemblyinfo :version do |asm|
    fv = version SemVer.find.to_s
    revision = (!fv[3] || fv[3] == 0) ? (ENV['BUILD_NUMBER'] || Time.now.strftime('%j%H')) : fv[3]
    ENV["BUILD_VERSION"] = BUILD_VERSION = "#{ SemVer.new(fv[0], fv[1], fv[2]).format "%M.%m.%p" }.#{revision}"
    
    asm.version = BUILD_VERSION
    asm.file_version = BUILD_VERSION
    
    asm.company_name = "Hadouken"
    asm.product_name = "Hadouken"
    asm.copyright = "2012"
    
    asm.output_file = "src/Shared/CommonAssemblyInfo.cs"
end

desc "Test"
nunit :test => :build do |nunit|
    FileUtils.mkdir_p "build/reports" unless FileTest.exists?("build/reports")
    
    nunit.command = "tools/nunit-2.6.0.12051/bin/nunit-console-x86.exe"
    nunit.options "/framework:v4.0.30319 /xml:build/reports/nunit.xml"
    nunit.assemblies "src/Tests/Hadouken.UnitTests/bin/Release/Hadouken.UnitTests.dll"
end

desc "Output"
task :output => :build do
    copy_files "src/Hosts/Hadouken.Hosts.CommandLine/bin/Release/", "*.{dll,exe}", "build/hdkn-#{BUILD_VERSION}"
    copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/Release/", "*.{dll,exe}", "build/hdkn-#{BUILD_VERSION}"
    copy_files "src/Config/Release/", "*.{config}", "build/hdkn-#{BUILD_VERSION}"    
end

desc "Zip"
zip :zip => :output do |zip|
    zip.directories_to_zip "build/hdkn-#{BUILD_VERSION}"
    zip.output_file = "hdkn-#{BUILD_VERSION}.zip"
    zip.output_path = "#{File.dirname(__FILE__)}/build/"
end

desc "Zip WebUI"
zip :zip_webui do |zip|
    zip.directories_to_zip "src/WebUI"
    zip.output_file = "webui.zip"
    zip.output_path = "#{File.dirname(__FILE__)}/build/hdkn-#{BUILD_VERSION}/"
end

desc "MSI"
task :msi => :output do
    puts "Hej"
    system "tools/wix-3.6rc/candle.exe -ext WixUtilExtension -dBuildVersion=#{BUILD_VERSION} -dBinDir=build/hdkn-#{BUILD_VERSION} -out src/Installer/ src/Installer/Hadouken.wxs src/Installer/SettingsDialog.wxs"
    system "tools/wix-3.6rc/light.exe -ext WixUIExtension -ext WixUtilExtension -sval -pdbout src/Installer/Hadouken.wixpdb -out build/hdkn-#{BUILD_VERSION}.msi src/Installer/Hadouken.wixobj src/Installer/SettingsDialog.wixobj"
end