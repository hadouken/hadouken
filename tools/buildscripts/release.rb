require 'semver'
require 'httparty'

namespace :release do
  task :common do    
    #ensure_clean_repo()
  end
  
  task :patch => :common do
    version = SemVer.find
    version.patch += 1
    
    ensure_version_available(version.to_s)
    
    version.save
    
    commit_and_tag(version.to_s)
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
  
  def commit_and_tag(v)
    `git commit .semver -m "Bumping version to #{v}"`
    `git tag #{v}`
  end
end