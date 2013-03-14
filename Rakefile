require "rubygems"
require "bundler"
Bundler.setup
$: << './'

require 'albacore'
require 'semver'
require 'rake/clean'

require 'tools/buildscripts/build'
require 'tools/buildscripts/environment'
require 'tools/buildscripts/utils'
require 'tools/buildscripts/msi'
require 'tools/buildscripts/output'
require 'tools/buildscripts/release'
require 'tools/buildscripts/test'
require 'tools/buildscripts/zip'

BUILD_ROOT = File.dirname(__FILE__)

CLOBBER.include("build/*")

task :default => [ :alpha ]

task :alpha => [ :clobber, "env:alpha", "env:release", "build:all", "test:all", "output:all", "zip:all", "msi:all" ]

task :beta => [ :clobber, "env:beta", "env:release", :build_x86, :reset, :build_x64 ]
task :rc => [ :clobber, "env:rc", "env:release", :build_x86, :reset, :build_x64 ]
task :ga => [ :clobber, "env:ga", "env:release", :build_x86, :reset, :build_x64 ]

desc "Versioning"
assemblyinfo :version => "env:common" do |asm|    
    asm.version = BUILD_VERSION
    asm.file_version = BUILD_VERSION
    
    asm.company_name = "Hadouken"
    asm.product_name = "Hadouken"
    asm.copyright = "2012"
    asm.namespaces = "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
    
    asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}", # disposed as product version in explorer
        :CLSCompliantAttribute => false,
        :AssemblyConfiguration => "#{CONFIGURATION}"
    
    asm.com_visible = false
    
    asm.output_file = "src/Shared/CommonAssemblyInfo.cs"
end