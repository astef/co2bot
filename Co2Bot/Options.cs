using Microsoft.Extensions.Configuration;

namespace Co2Bot
{
    public class Options
    {
        public int Frequency { get; private set; }

        public int LongRepeatDelayMin { get; private set; }
        public int ShortRepeatCount { get; private set; }

        public string Co2DataPath { get; private set; }
        public bool SendToConsole { get; private set; }

        public int RedCo2 { get; private set; }
        public int YellowCo2 { get; private set; }

        public double RedTemp { get; private set; }
        public double BlueTemp { get; private set; }

        public string HttpUrl { get; private set; }

        public string MorningTime { get; private set; }

        public string EveningTime { get; private set; }

        public Options Configure(IConfiguration cfg)
        {
            Frequency = cfg.GetValue<int>(nameof(Frequency));
            LongRepeatDelayMin = cfg.GetValue<int>(nameof(LongRepeatDelayMin));
            ShortRepeatCount = cfg.GetValue<int>(nameof(ShortRepeatCount));
            Co2DataPath = cfg[nameof(Co2DataPath)];
            SendToConsole = cfg.GetValue<bool>(nameof(SendToConsole));
            RedCo2 = cfg.GetValue<int>(nameof(RedCo2));
            YellowCo2 = cfg.GetValue<int>(nameof(YellowCo2));
            RedTemp = cfg.GetValue<double>(nameof(RedTemp));
            BlueTemp = cfg.GetValue<double>(nameof(BlueTemp));
            HttpUrl = cfg[nameof(HttpUrl)];
            MorningTime = cfg.GetValue<string>(nameof(MorningTime));
            EveningTime = cfg.GetValue<string>(nameof(EveningTime));
            return this;
        }
    }
}