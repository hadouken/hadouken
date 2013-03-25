require 'semver'
require 'psych'
require 'httparty'
require 'net/sftp'

CFG = YAML.load_file("config/config.yml") unless defined? CFG

namespace :release do
  task :common do
    ensure_clean_repo()
    ensure_correct_branch()
  end
  
  task :major => :common do
    version = SemVer.find
    version.major += 1
    version.minor = version.patch = 0
    version.save
    
    Rake::Task["ga"].invoke
  end
  
  task :minor => :common do
    version = SemVer.find
    version.minor += 1
    version.patch = 0
    version.save
    
    Rake::Task["ga"].invoke
  end
  
  task :patch => :common do
    version = SemVer.find
    version.patch += 1
    version.save
    
    Rake::Task["ga"].invoke
  end
  
  task :publish => :common do
    version = SemVer.find
    
    ensure_msi_packages(version.format("%M.%m.%p"))
    ensure_version_available(version.to_s)
    
    copy_to_server("build/msi/hdkn-#{version.format("%M.%m.%p")}.4000-x86.msi", version.format("%M.%m"))
    copy_to_server("build/msi/hdkn-#{version.format("%M.%m.%p")}.4000-x64.msi", version.format("%M.%m"))
    
    commit_repo(version.to_s)
    tag_repo(version.to_s)
    push_repo(CFG["git"]["remote"], version.to_s)
  end
  
  def ensure_msi_packages(v)
    x86 = "build/msi/hdkn-#{v}.4000-x86.msi"
    x64 = "build/msi/hdkn-#{v}.4000-x64.msi"
    
    puts "checking for #{x86} and #{x64}"
    
    unless File.exist?(x86)
      fail "x86 msi package does not exist"
    end
    
    unless File.exist?(x64)
      fail "x64 msi package does not exist"
    end
  end
  
  def ensure_correct_branch()
    release_branch = CFG["git"]["release_branch"]
    
    puts "checking current branch (must be #{release_branch})"
    branch = `git rev-parse --abbrev-ref HEAD`.strip
    
    if branch != release_branch then
      fail "can only release from branch #{release_branch} (current branch is #{branch})"
    end
  end
  
  def ensure_clean_repo()
    puts "checking if we have a clean repo tree"
    
    if `git status --porcelain` != "" then
      fail "dirty repository tree"
    end
    
    puts "repository OK"
  end
  
  def ensure_version_available(v)
    puts "checking with github if we have released #{v} already"
    
    response = HTTParty.get(CFG["github"]["api"]["tags"])
    
    response.each do |tag|
      tv = version(tag["name"])
      lv = version(v)
      
      if(tv[0] > lv[0])
        fail "newer version already released. github tag: #{tag["name"]}, version: #{v}"
      end
      
      if(tv[1] > lv[1])
        fail "newer version already released. github tag: #{tag["name"]}, version: #{v}"
      end
      
      if(tv[2] >= lv[2])
        fail "newer version already released. github tag: #{tag["name"]}, version: #{v}"
      end
    end
    
    puts "version OK"
  end
  
  def copy_to_server(localFile, v)
    puts "copying file #{localFile} to remote server"
    
    s = CFG["sftp"]
    
    Net::SFTP.start(s["host"], s["user"], :keys => s["keys"]) do |sftp|
      hasDir = false
      
      sftp.dir.foreach(s["path"]) do |p|
        if(p.name == v)
          puts "directory #{File.join(s["path"], v)} already exists"
          hasDir = true
        end
      end
      
      unless hasDir
        puts "creating remote directory #{File.join(s["path"], v)}"
        sftp.mkdir!(File.join(s["path"], v))
      end
      
      puts "uploading file"
      sftp.upload!(localFile, File.join(s["path"], v, File.basename(localFile)))
    end
  end
  
  def commit_repo(v)
    `git commit .semver -m "Release #{v}"`
  end
  
  def tag_repo(v)
    puts "tagging repo with version #{v}"
    `git tag #{v}`
  end
  
  def push_repo(remote, tag)
    puts "pushing tag #{tag} to remote repository #{remote}"
    `git push #{remote}`
    `git push #{remote} #{tag}`
  end
end