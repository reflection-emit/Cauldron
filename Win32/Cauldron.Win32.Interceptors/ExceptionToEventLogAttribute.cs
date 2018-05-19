using Cauldron.Interception;
using System;
using System.Diagnostics;
using System.Reflection;
using Cauldron.Core.Diagnostics;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Provides an interceptor that logs an exception to the Windows Event Log.
    /// </summary>
    [InterceptorOptions(AlwaysCreateNewInstance = true)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExceptionToEventLogAttribute : Attribute, IPropertyInterceptor, IMethodInterceptor
    {
        private short? category;
        private int? eventId;
        private EventLogEntryType eventLogEntryType = EventLogEntryType.Error;
        private string logName;

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionToEventLogAttribute"/>.
        /// </summary>
        /// <param name="logname">The source name to register and use when writing to the event log.</param>
        /// <param name="eventLogEntryType">One of the event log entry type value.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        public ExceptionToEventLogAttribute(string logname, EventLogEntryType eventLogEntryType, int eventId, short category)
        {
            this.logName = logname;
            this.eventLogEntryType = eventLogEntryType;
            this.eventId = eventId;
            this.category = category;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionToEventLogAttribute"/>.
        /// </summary>
        /// <param name="logname">The source name to register and use when writing to the event log.</param>
        public ExceptionToEventLogAttribute(string logname)
        {
            this.logName = logname;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionToEventLogAttribute"/>.
        /// </summary>
        /// <param name="logname">The source name to register and use when writing to the event log.</param>
        /// <param name="eventLogEntryType">One of the event log entry type value.</param>
        public ExceptionToEventLogAttribute(string logname, EventLogEntryType eventLogEntryType)
        {
            this.logName = logname;
            this.eventLogEntryType = eventLogEntryType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionToEventLogAttribute"/>.
        /// </summary>
        /// <param name="eventLogEntryType">One of the event log entry type value.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        public ExceptionToEventLogAttribute(EventLogEntryType eventLogEntryType, int eventId, short category)
        {
            this.logName = LogName;
            this.eventLogEntryType = eventLogEntryType;
            this.eventId = eventId;
            this.category = category;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionToEventLogAttribute"/>.
        /// </summary>
        /// <param name="eventLogEntryType">One of the event log entry type value.</param>
        public ExceptionToEventLogAttribute(EventLogEntryType eventLogEntryType)
        {
            this.logName = LogName;
            this.eventLogEntryType = eventLogEntryType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionToEventLogAttribute"/>.
        /// </summary>
        public ExceptionToEventLogAttribute()
        {
            this.logName = LogName;
        }

        /// <summary>
        /// Gets or sets the default source name to register and use when writing to the event log.
        /// </summary>
        public static string LogName { get; set; } = "Application";

        /// <exclude/>
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        /// <exclude/>
        public bool OnException(Exception e)
        {
            using (var eventLog = new EventLog(this.logName, Environment.MachineName))
            {
                eventLog.Source = this.logName;

                if (this.eventId.HasValue)
                    eventLog.WriteEntry(e.GetStackTrace(), this.eventLogEntryType, this.eventId.Value, this.category.Value);
                else
                    eventLog.WriteEntry(e.GetStackTrace(), this.eventLogEntryType);
            }

            return true;
        }

        /// <exclude/>
        public void OnExit()
        {
        }

        /// <exclude/>
        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
        }

        /// <exclude/>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue) => false;
    }
}