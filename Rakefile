require "rubygems"
require "bundler/setup"

require "albacore"

desc "Build"
msbuild :build do |msb|
    msb.properties :configuration => "Release"
    msb.targets :Clean, :Build
    msb.solution = "Hadouken.sln"
end