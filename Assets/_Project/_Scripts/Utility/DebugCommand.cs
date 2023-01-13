using UnityEngine;
using System;

namespace MyTownProject.Utility{

    public class DebugCommandBase{

        string _commandId;
        string _commandDescription;
        string _commandFormat;
        
        public string CommandId {get{return _commandId;}}
        public string CommandDescription {get{return _commandDescription;}}
        public string CommandFormat {get{return _commandFormat;}}

        public DebugCommandBase(string id, string description, string format){
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;
        }

    }

    public class DebugCommand: DebugCommandBase{

        public Action Command;

        public DebugCommand(string id, string description, string format, Action command) : base(id, description, format){
            this.Command = command;
        }

        public void Invoke() => Command.Invoke();
    }

    public class DebugCommand<T1>: DebugCommandBase{

        public Action<T1> Command;

        public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format){
            this.Command = command;
        }

        public void Invoke(T1 value) => Command.Invoke(value);
    }
    public class DebugCommand<T1, T2>: DebugCommandBase{

       public Action<T1, T2> Command;

       public DebugCommand(string id, string description, string format, Action<T1, T2> command) : base(id, description, format){
           this.Command = command;
       }

       public void Invoke(T1 value1, T2 value2) => Command.Invoke(value1, value2);
    }

}