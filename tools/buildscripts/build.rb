

namespace :build do
    def compile(msb, platform)
        puts "##teamcity[progressMessage 'Compiling #{platform} code']"
        
        msb.properties :configuration => :Release
        msb.properties :platform      => platform
        msb.targets :Clean, :Build
        
        msb.solution = "Hadouken.sln"
    end
    
    msbuild :x86 => :version do |msb|
        compile msb, :x86
    end
    
    msbuild :x64 => :version do |msb|
        compile msb, :x64
    end
    
    task :all do
        Rake::Task["build:x86"].invoke
        Rake::Task["build:x64"].invoke
    end
end