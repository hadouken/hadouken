

namespace :build do
    msbuild :x86 => :version do |msb|
        puts "##teamcity[progressMessage 'Compiling x86 code']"
        
        msb.properties :configuration => :Release
        msb.properties :platform      => :x86
        msb.targets :Clean, :Build
        
        msb.solution = "Hadouken.sln"
    end
    
    msbuild :x64 => :version do |msb|
        puts "##teamcity[progressMessage 'Compiling x64 code']"
        
        msb.properties :configuration => :Release
        msb.properties :platform      => :x64
        msb.targets :Clean, :Build
        
        msb.solution = "Hadouken.sln"
    end
    
    task :all do
        Rake::Task["build:x86"].invoke
        Rake::Task["build:x64"].invoke
    end
end