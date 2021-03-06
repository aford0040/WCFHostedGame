﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheConsole.BroadcastorService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EventDataType", Namespace="http://schemas.datacontract.org/2004/07/BroadcastorService")]
    [System.SerializableAttribute()]
    public partial class EventDataType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ClientNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private TheConsole.BroadcastorService.BroadcastorServiceEvent CurrentEventField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EventMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] GameBoardField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ClientName {
            get {
                return this.ClientNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ClientNameField, value) != true)) {
                    this.ClientNameField = value;
                    this.RaisePropertyChanged("ClientName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public TheConsole.BroadcastorService.BroadcastorServiceEvent CurrentEvent {
            get {
                return this.CurrentEventField;
            }
            set {
                if ((this.CurrentEventField.Equals(value) != true)) {
                    this.CurrentEventField = value;
                    this.RaisePropertyChanged("CurrentEvent");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EventMessage {
            get {
                return this.EventMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.EventMessageField, value) != true)) {
                    this.EventMessageField = value;
                    this.RaisePropertyChanged("EventMessage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] GameBoard {
            get {
                return this.GameBoardField;
            }
            set {
                if ((object.ReferenceEquals(this.GameBoardField, value) != true)) {
                    this.GameBoardField = value;
                    this.RaisePropertyChanged("GameBoard");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BroadcastorService.Event", Namespace="http://schemas.datacontract.org/2004/07/BroadcastorService")]
    public enum BroadcastorServiceEvent : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Registration = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SendMessage = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UpdateGameBoard = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BroadcastorService.IBroadcastorService", CallbackContract=typeof(TheConsole.BroadcastorService.IBroadcastorServiceCallback))]
    public interface IBroadcastorService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBroadcastorService/RegisterClient")]
        void RegisterClient(string clientName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBroadcastorService/NotifyServer")]
        void NotifyServer(TheConsole.BroadcastorService.EventDataType eventData);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBroadcastorService/AddPlayer")]
        void AddPlayer(string clientName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBroadcastorService/GenerateGameBoard")]
        void GenerateGameBoard();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBroadcastorService/UpdateGameBoard")]
        void UpdateGameBoard(string[] RecievedGameBoard);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBroadcastorServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBroadcastorService/BroadcastToClient")]
        void BroadcastToClient(TheConsole.BroadcastorService.EventDataType eventData);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBroadcastorServiceChannel : TheConsole.BroadcastorService.IBroadcastorService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BroadcastorServiceClient : System.ServiceModel.DuplexClientBase<TheConsole.BroadcastorService.IBroadcastorService>, TheConsole.BroadcastorService.IBroadcastorService {
        
        public BroadcastorServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public BroadcastorServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public BroadcastorServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public BroadcastorServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public BroadcastorServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void RegisterClient(string clientName) {
            base.Channel.RegisterClient(clientName);
        }
        
        public void NotifyServer(TheConsole.BroadcastorService.EventDataType eventData) {
            base.Channel.NotifyServer(eventData);
        }
        
        public void AddPlayer(string clientName) {
            base.Channel.AddPlayer(clientName);
        }
        
        public void GenerateGameBoard() {
            base.Channel.GenerateGameBoard();
        }
        
        public void UpdateGameBoard(string[] RecievedGameBoard) {
            base.Channel.UpdateGameBoard(RecievedGameBoard);
        }
    }
}
