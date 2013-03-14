namespace :test do
    def run_tests(platform)
        puts "##teamcity[progressMessage 'Running unit tests for #{platform} code']"
        FileUtils.mkdir_p "build/reports" unless FileTest.exists?("build/reports")
        
        if(ENV['TEAMCITY_VERSION'])
            system "#{ENV['NUNIT_LAUNCHER']} v4.0 #{platform} NUnit-2.6.0 src/Tests/Hadouken.UnitTests/bin/#{platform}/#{CONFIGURATION}/Hadouken.UnitTests.dll"
        else
            system "tools/nunit-2.6.0.12051/bin/nunit-console#{platform == :x86 ? "-x86" : ""}.exe /framework:v4.0.30319 /xml:build/reports/nunit-#{platform}.xml src/Tests/Hadouken.UnitTests/bin/#{platform}/#{CONFIGURATION}/Hadouken.UnitTests.dll"
        end
    end

    task :x86 => "build:x86" do
        run_tests(:x86)
    end
    
    task :x64 => "build:x64" do
        run_tests(:x64)
    end
    
    task :all do
        Rake::Task["test:x86"].invoke
        Rake::Task["test:x64"].invoke
    end
end