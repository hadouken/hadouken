namespace :test do
    task :x86 => "build:x86" do
        puts "##teamcity[progressMessage 'Running unit tests for x86 code']"
        FileUtils.mkdir_p "build/reports" unless FileTest.exists?("build/reports")
        
        if(ENV['TEAMCITY_VERSION'])
            system "#{ENV['NUNIT_LAUNCHER']} v4.0 x86 NUnit-2.6.0 src/Tests/Hadouken.UnitTests/bin/x86/#{CONFIGURATION}/Hadouken.UnitTests.dll"
        else
            system "tools/nunit-2.6.0.12051/bin/nunit-console-x86.exe /framework:v4.0.30319 /xml:build/reports/nunit-x86.xml src/Tests/Hadouken.UnitTests/bin/x86/#{CONFIGURATION}/Hadouken.UnitTests.dll"
        end
    end
    
    task :x64 => "build:x64" do
        puts "##teamcity[progressMessage 'Running unit tests for x86 code']"
        FileUtils.mkdir_p "build/reports" unless FileTest.exists?("build/reports")
        
        if(ENV['TEAMCITY_VERSION'])
            system "#{ENV['NUNIT_LAUNCHER']} v4.0 x64 NUnit-2.6.0 src/Tests/Hadouken.UnitTests/bin/x64/#{CONFIGURATION}/Hadouken.UnitTests.dll"
        else
            system "tools/nunit-2.6.0.12051/bin/nunit-console.exe /framework:v4.0.30319 /xml:build/reports/nunit-x64.xml src/Tests/Hadouken.UnitTests/bin/x64/#{CONFIGURATION}/Hadouken.UnitTests.dll"
        end
    end
    
    task :all do
        Rake::Task["test:x86"].invoke
        Rake::Task["test:x64"].invoke
    end
end