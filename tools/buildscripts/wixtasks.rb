class Candle
  include Albacore::Task
  include Albacore::RunCommand

  attr_accessor :sources
  attr_accessor :out
  attr_accessor :extensions
  attr_hash :defines

  def execute
    run_command "Candle", "#{@extensions.map{|k| "-ext #{k}"}.join(' ')} #{@out ? '-out ' + @out : ''} #{@defines.map{|k,v| "-d#{k}=#{v}"}.join(' ')} #{@sources.join(' ')}"
  end
end

class Light
  include Albacore::Task
  include Albacore::RunCommand

  attr_accessor :sources
  attr_accessor :pdbout
  attr_accessor :out
  attr_accessor :extensions

  def execute
    puts @extensions
    run_command "Light", "#{@extensions.map{|k| "-ext #{k}"}.join(' ')} #{@pdbout ? '-pdbout ' + @pdbout : ''} #{@out ? '-out ' + @out : ''} #{@sources.join(' ')}"
  end
end