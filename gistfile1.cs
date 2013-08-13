    /// <summary>
    /// Performs tracing of INotifyPropertyChange events
    /// </summary>
    public class INCPTracer
    {

        ~INCPTracer()
        {
            Detach();
            Clear();
        }

        public INCPTracer With(INotifyPropertyChanged notifier)
        {
            Detach();
            Notifier = notifier;
            Attach();
            Clear();
           
            return this;
        }


        public INCPTracer CheckThat(Action a)
        {
            a();
            return this;
        }

        /// <summary>
        /// Call this method multiple times if an action created multiple events. The order will be preserved
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public INCPTracer RaisedEvent<T>(Expression<Func<T>> expression)
        {

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var body = expression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("The body must be a member expression");
            }


            if (EventPos >= RaisedEventList.Count)
            {
                throw new ArgumentException(string.Format("Expected event '{0}' but no more events left", body.Member.Name));
            }

            if (RaisedEventList[EventPos] != body.Member.Name)
            {
                throw new ArgumentException(string.Format("Event {0} was not at position {1}", body.Member.Name, EventPos));
            }

            ++EventPos;
            return this;
        }



        private void NotifierOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RaisedEventList.Add(propertyChangedEventArgs.PropertyName);
        }


        protected void Detach()
        {
            if (Notifier != null) 
            { 
                Notifier.PropertyChanged -= NotifierOnPropertyChanged; 
            }
        }

        protected void Attach()
        {
            if (Notifier != null)
            {
                Notifier.PropertyChanged += NotifierOnPropertyChanged;
            }
            
        }

        protected void Clear()
        {
            RaisedEventList.Clear();
            EventPos = 0;
        }

        protected INotifyPropertyChanged Notifier;
        protected IList<string> RaisedEventList = new List<string>();
        protected int EventPos = 0;
    }