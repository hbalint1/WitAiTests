using System.Collections.Generic;

namespace Wit.Communication.WitAiComm
{
    public class WitAiResponse
    {
        public string msg_id { get; set; }
        public string _text { get; set; }
        public Entities entities { get; set; }

        public class Intent
        {
            public double confidence { get; set; }
            public string value { get; set; }
        }

        public class OnOff
        {
            public double confidence { get; set; }
            public string value { get; set; }
        }

        public class Entities
        {
            public List<Intent> intent { get; set; }
            public List<OnOff> on_off { get; set; }

            public override string ToString()
            {
                return intent?.ToString() + on_off?.ToString();
            }
        }
    }
}