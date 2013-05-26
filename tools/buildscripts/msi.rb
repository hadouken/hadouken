require 'tools/buildscripts/wixtasks'

namespace :msi do
    candle :x86_candle => "output:x86" do |cndl|
        puts "##teamcity[progressMessage 'Building x86 MSI package']"
        
        cndl.command = "tools/wix-3.7/candle.exe"
        cndl.defines(
            :Name => "Hadouken",
            :BinDir => "build/bin/hdkn-#{BUILD_VERSION}-x86",
            :Platform => :x86,
            :BuildVersion => BUILD_VERSION
        )
        cndl.extensions = [ :WixFirewallExtension, :WixUtilExtension, :WixUIExtension, :WiXNetFxExtension ]
        cndl.sources =  [
                            "src/Installer/Hadouken.wxs",
                            "src/Installer/Components/Config.wxs",
                            "src/Installer/Components/Core.wxs",
                            "src/Installer/Components/Firewall.wxs",
                            "src/Installer/Components/Lib.wxs",
                            "src/Installer/Components/Service.wxs",
                            "src/Installer/UI/ConfigDialog.wxs",
                            "src/Installer/UI/ConfigInvalidDialog.wxs"
                        ]
        cndl.out = "src/Installer/obj/x86/"
    end
    
    light :x86_light => "msi:x86_candle" do |lght|
        lght.command = "tools/wix-3.7/light.exe"
        lght.extensions = [ :WixFirewallExtension, :WixUtilExtension, :WixUIExtension, :WiXNetFxExtension ]
        lght.pdbout = "src/Installer/obj/x86/pdb/Hadouken.wixpdb"
        lght.sources = Dir.glob("src/Installer/obj/x86/*.wixobj")
        lght.out = "build/msi/hdkn-#{BUILD_VERSION}-x86.msi"
    end
    
    task :x86 do
        Rake::Task["msi:x86_light"].invoke
    end
    
    candle :x64_candle => "output:x64" do |cndl|
        puts "##teamcity[progressMessage 'Building x64 MSI package']"
        
        cndl.command = "tools/wix-3.7/candle.exe"
        cndl.defines(
            :Name => "Hadouken",
            :BinDir => "build/bin/hdkn-#{BUILD_VERSION}-x64",
            :Platform => :x64,
            :BuildVersion => BUILD_VERSION
        )
        cndl.extensions = [ :WixFirewallExtension, :WixUtilExtension, :WixUIExtension, :WiXNetFxExtension ]
        cndl.sources =  [
                            "src/Installer/Hadouken.wxs",
                            "src/Installer/Components/Config.wxs",
                            "src/Installer/Components/Core.wxs",
                            "src/Installer/Components/Firewall.wxs",
                            "src/Installer/Components/Lib.wxs",
                            "src/Installer/Components/Service.wxs",
                            "src/Installer/UI/ConfigDialog.wxs",
                            "src/Installer/UI/ConfigInvalidDialog.wxs"
                        ]
                        
        cndl.out = "src/Installer/obj/x64/"
    end
    
    light :x64_light => "msi:x64_candle" do |lght|
        lght.command = "tools/wix-3.7/light.exe"
        lght.extensions = [ :WixFirewallExtension, :WixUtilExtension, :WixUIExtension, :WiXNetFxExtension ]
        lght.pdbout = "src/Installer/obj/x64/pdb/Hadouken.wixpdb"
        lght.sources = Dir.glob("src/Installer/obj/x64/*.wixobj")
        lght.out = "build/msi/hdkn-#{BUILD_VERSION}-x64.msi"
    end
    
    task :x64 do
        Rake::Task["msi:x64_light"].invoke
    end
    
    task :all do
        Rake::Task["msi:x86"].invoke
        Rake::Task["msi:x64"].invoke
    end
end