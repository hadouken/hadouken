namespace :output do
    def do_copy(platform)
        puts "##teamcity[progressMessage 'Outputting #{platform} binaries']"
        
        copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/#{platform}/#{CONFIGURATION}/", "*.{dll,exe}", "build/bin/hdkn-#{BUILD_VERSION}-#{platform}"
        copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/#{platform}/#{CONFIGURATION}/#{platform}/", "*.{dll,exe}", "build/bin/hdkn-#{BUILD_VERSION}-#{platform}"
        
        copy_files "src/Config/#{CONFIGURATION}/", "*.{config}", "build/bin/hdkn-#{BUILD_VERSION}-#{platform}"
    end
    
    task :x86 => "test:x86" do
        do_copy(:x86)
    end
        
    task :x64 => "test:x64" do
        do_copy(:x64)
    end
    
    task :all do
        Rake::Task["output:x86"].invoke
        Rake::Task["output:x64"].invoke
    end
end