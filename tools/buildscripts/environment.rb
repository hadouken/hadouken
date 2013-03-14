require 'semver'

namespace :env do
    # version setup
    task :common do
        fv = version(SemVer.find.to_s)

        ENV['BUILD_VERSION'] = BUILD_VERSION = "#{ SemVer.new(fv[0], fv[1], fv[2]).format "%M.%m.%p" }.#{REVISION}"
        puts "##teamcity[buildNumber '#{BUILD_VERSION}']" # tell teamcity our version
    end
    
    task :configure, [:str] do |_, args|
        ENV['CONFIGURATION'] = CONFIGURATION = args[:str]
    end
    
    desc "set debug env variables"
    task :debug => [:common] do
        Rake::Task["env:configure"].invoke('Debug')
    end
    
    desc "set releaseenv variables"
    task :release => [:common] do
        Rake::Task["env:configure"].invoke('Release')
    end
    
    desc "Set GA env vars"
    task :ga do
        puts "##teamcity[progressMessage 'Setting environment variables for GA']"
        ENV['REVISION'] = REVISION = "4000"
    end
    
    desc "Set RC env vars"
    task :rc do
        puts "##teamcity[progressMessage 'Setting environment variables for Release Candidate']"
        ENV['REVISION'] = REVISION = (ENV['BUILD_NUMBER'] ? (3000 + ENV['BUILD_NUMBER'].to_i).to_s : "3000")
    end
    
    desc "Set beta env vars"
    task :beta do
        puts "##teamcity[progressMessage 'Setting environment variables for Beta']"
        ENV['REVISION'] = REVISION = (ENV['BUILD_NUMBER'] ? (2000 + ENV['BUILD_NUMBER'].to_i).to_s : "2000")
    end
    
    desc "Set alpha env vars"
    task :alpha do
        puts "##teamcity[progressMessage 'Setting environment variables for Alpha']"
        ENV['REVISION'] = REVISION = (ENV['BUILD_NUMBER'] ? (1000 + ENV['BUILD_NUMBER'].to_i).to_s : "1000")
    end
end

namespace :arch do
    #x86
    task :x86 do
        ENV['BUILD_PLATFORM'] = BUILD_PLATFORM = 'x86'
    end
    
    #x64
    task :x64 do
        Object.send(:remove_const, :BUILD_PLATFORM)
        ENV['BUILD_PLATFORM'] = BUILD_PLATFORM = 'x64'
    end
end