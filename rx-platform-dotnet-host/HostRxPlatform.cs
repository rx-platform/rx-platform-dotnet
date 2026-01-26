namespace ENSACO.RxPlatform.Hosting.Internal
{
    
    public enum rx_item_type : byte
    {
        rx_directory = 0,
        rx_application = 1,
        rx_application_type = 2,
        rx_domain = 3,
        rx_domain_type = 4,
        rx_object = 5,
        rx_object_type = 6,
        rx_port = 7,
        rx_port_type = 8,
        rx_struct_type = 9,
        rx_variable_type = 10,
        rx_source_type = 11,
        rx_filter_type = 12,
        rx_event_type = 13,
        rx_mapper_type = 14,
        rx_relation_type = 15,
        rx_program_type = 16,
        rx_method_type = 17,
        rx_data_type = 18,
        rx_display_type = 19,
        rx_relation = 20,

        rx_first_invalid = 21,

        rx_test_case_type = 0xfe,
        rx_invalid_type = 0xff
    }
    internal class RxPlatformObject
    {
        protected RxPlatformObject()
        {

        }
        private static RxPlatformObject? instance = null;

        public static RxPlatformObject Instance
        { 
            get
            {
                if(instance == null)
                    instance = new RxPlatformObject();
                return instance;
            }
        } 

        enum log_event_type : int
        {
            debug = 0,
	        trace = 1,
	        info = 2,
	        warning = 3,
	        error = 4,
	        critical = 5
        };
        internal string GetPluginName()
        {
            return "dotnet";
        }
        public void WriteLogInfo(string source, ushort severity, string message)
        {
            if (PlatformHostMain.api.WriteLog != null)
            {
                PlatformHostMain.api.WriteLog((int)log_event_type.info, GetPluginName(), source, severity, "", message);
            }
        }
        public void WriteLogError(string source, ushort severity, string message)
        {
            if (PlatformHostMain.api.WriteLog != null)
            {
                PlatformHostMain.api.WriteLog((int)log_event_type.error, GetPluginName(), source, severity, "", message);
            }
        }
        public void WriteLogWarning(string source, ushort severity, string message)
        {
            if (PlatformHostMain.api.WriteLog != null)
            {
                PlatformHostMain.api.WriteLog((int)log_event_type.warning, GetPluginName(), source, severity, "", message);
            }
        }
        public void WriteLogDebug(string source, ushort severity, string message)
        {
            if (PlatformHostMain.api.WriteLog != null)
            {
                PlatformHostMain.api.WriteLog((int)log_event_type.debug, GetPluginName(), source, severity, "", message);
            }
        }
        public void WriteLogTrace(string source, ushort severity, string message)
        {
            if (PlatformHostMain.api.WriteLog != null)
            {
                PlatformHostMain.api.WriteLog((int)log_event_type.trace, GetPluginName(), source, severity, "", message);
            }
        }
        public void WriteLogCritical(string source, ushort severity, string message)
        {
            if (PlatformHostMain.api.WriteLog != null)
            {
                PlatformHostMain.api.WriteLog((int)log_event_type.critical, GetPluginName(), source, severity, "", message);
            }
        }
    }
}
