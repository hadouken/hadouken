namespace :zip do
    zip :webui_x86 do |zip|
        puts "##teamcity[progressMessage 'Zipping webui for x86']"
        
        zip.directories_to_zip "src/WebUI"
        zip.output_file = "webui.zip"
        zip.output_path = "#{BUILD_ROOT}/build/bin/hdkn-#{BUILD_VERSION}-x86/"
    end
    
    zip :webui_x64 do |zip|
        puts "##teamcity[progressMessage 'Zipping webui for x64']"
        
        zip.directories_to_zip "src/WebUI"
        zip.output_file = "webui.zip"
        zip.output_path = "#{BUILD_ROOT}/build/bin/hdkn-#{BUILD_VERSION}-x64/"
    end

    zip :x86 => "output:x86" do |zip|
        puts "##teamcity[progressMessage 'Zipping x86 binaries']"
        FileUtils.mkdir_p "build/zip" unless FileTest.exists?("build/zip")
        
        zip.directories_to_zip "build/bin/hdkn-#{BUILD_VERSION}-x86"
        zip.output_file = "hdkn-#{BUILD_VERSION}-x86.zip"
        zip.output_path = "#{BUILD_ROOT}/build/zip/"
    end
    
    zip :x64 => "output:x64" do |zip|
        puts "##teamcity[progressMessage 'Zipping x64 binaries']"
        FileUtils.mkdir_p "build/zip" unless FileTest.exists?("build/zip")
        
        zip.directories_to_zip "build/bin/hdkn-#{BUILD_VERSION}-x64"
        zip.output_file = "hdkn-#{BUILD_VERSION}-x64.zip"
        zip.output_path = "#{BUILD_ROOT}/build/zip/"
    end
    
    task :all do
        Rake::Task["zip:webui_x86"].invoke
        Rake::Task["zip:webui_x64"].invoke
        Rake::Task["zip:x86"].invoke
        Rake::Task["zip:x64"].invoke
    end
end