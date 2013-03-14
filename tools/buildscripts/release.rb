require 'semver'
require 'httparty'

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
    
    `git commit .semver -m "Release #{version.to_s}"`
    
    Rake::Task["ga"].invoke
  end
  
  task :minor => :common do
    version = SemVer.find
    version.minor += 1
    version.patch = 0
    version.save
    
    `git commit .semver -m "Release #{version.to_s}"`
    
    Rake::Task["ga"].invoke
  end
  
  task :patch => :common do
    version = SemVer.find
    version.patch += 1
    version.save
    
    `git commit .semver -m "Release #{version.to_s}"`
    
    Rake::Task["ga"].invoke
  end
  
  task :publish => :common do
    version = SemVer.find
    
    ensure_msi_packages(version.format("%M.%m.%p"))
    ensure_version_available(version.to_s)
    
    tag_repo(version.to_s)
    push_repo(:origin, version.to_s)
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
    puts "checking current branch (must be master)"
    branch = `git rev-parse --abbrev-ref HEAD`.strip
    
    if branch != "releaseflow" then
      fail "can only release from master branch (current branch is #{branch})"
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
    
    response = HTTParty.get("https://api.github.com/repos/hadouken/hdkn/tags")
    
    response.each do |tag|
      tv = version(tag["name"])
      lv = version(v)
      
      if(tv[0] > lv[0])
        fail "newer version already released"
      end
      
      if(tv[1] > lv[1])
        fail "newer version already released"
      end
      
      if(tv[2] >= lv[2])
        fail "newer version already released"
      end
    end
    
    puts "version OK"
  end
  
  def tag_repo(v)
    puts "tagging repo with version #{v}"
    `git tag #{v}`
  end
  
  def push_repo(remote, tag)
    puts "pushing tag #{tag} to remote repository #{remote}"
    `git push test #{tag}`
  end
end