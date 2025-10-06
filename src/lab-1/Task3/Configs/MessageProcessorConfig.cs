using System.Threading.Channels;

namespace Task3.Configs;

public class MessageProcessorConfig
{
    public int ChannelCapacity { get; set; } = 100;

    public int BatchVolume { get; set; } = 10;

    public TimeSpan BatchTime { get; set; } = TimeSpan.FromMilliseconds(100);

    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.Wait;

    public bool SingleReader { get; set; } = true;

    public bool SingleWriter { get; set; } = false;
}