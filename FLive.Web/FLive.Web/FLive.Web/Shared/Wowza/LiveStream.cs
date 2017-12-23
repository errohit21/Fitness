namespace FLive.Web.Shared.Wowza
{
    public class live_stream
    {
        public string id { get; set; }
        public string name { get; set; }

        public string broadcast_location { get; set; }
        public string transcoder_type => "transcoded";
        public string billing_mode => "pay_as_you_go";
        public string encoder => "other_rtmp";
        public string delivery_method = "push";
        public int aspect_ratio_width => 640;
        public int aspect_ratio_height => 480;

        public bool recording => true;
        public bool player_countdown => true;

        /*   label: "352x288     (280 kbps)",
                      value: '352x288'
                      }, {
                      label: "640x480     (1.5 mbps)",
                      value: '640x480'
                      }, {
                      label: "1280x720     (3.75 mbps)",
                      value: '1280x720'
                      }, {
                      label: "1920x1080     (5 mbps)",
                      value: '1920x1080'
                      }, {
                      label: "3840x2160     (8 mbps)",
                      value: '3840x2160'*/
    }
}