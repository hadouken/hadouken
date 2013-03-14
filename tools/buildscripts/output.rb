namespace :output do
    task :x86 => "test:x86" do
        puts "##teamcity[progressMessage 'Outputting x86 binaries']"
        
        copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/x86/#{CONFIGURATION}/", "*.{dll,exe}", "build/bin/hdkn-#{BUILD_VERSION}-x86"
        copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/x86/#{CONFIGURATION}/x86/", "*.{dll,exe}", "build/bin/hdkn-#{BUILD_VERSION}-x86"
        
        copy_files "src/Config/#{CONFIGURATION}/", "*.{config}", "build/bin/hdkn-#{BUILD_VERSION}-x86"
    end
    
    task :x64 => "test:x64" do
        puts "##teamcity[progressMessage 'Outputting x64 binaries']"
        
        copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/x64/#{CONFIGURATION}/", "*.{dll,exe}", "build/bin/hdkn-#{BUILD_VERSION}-x64"
        copy_files "src/Hosts/Hadouken.Hosts.WindowsService/bin/x64/#{CONFIGURATION}/x64/", "*.{dll,exe}", "build/bin/hdkn-#{BUILD_VERSION}-x64"
        
        copy_files "src/Config/#{CONFIGURATION}/", "*.{config}", "build/bin/hdkn-#{BUILD_VERSION}-x64"
    end
    
    task :all do
        Rake::Task["output:x86"].invoke
        Rake::Task["output:x64"].invoke
    end
end