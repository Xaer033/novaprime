using System;
using UnityEngine;
//
// [RequireComponent(typeof(BoltEntity))]
// public class NetworkEntity : Bolt.EntityBehaviour<IState>
// {                   
//     public event Action<NetworkEntity> onAttached;                  
//     public event Action<NetworkEntity> onDetached;                  
//     public event Action<NetworkEntity> onSimulateOwner;                 
//     public event Action<NetworkEntity> onSimulateController;
//     public event Action<NetworkEntity> onControlGained;
//     public event Action<NetworkEntity> onControlLost;
//     public event Action<NetworkEntity, Command> onMissingCommand;
//     public event Action<NetworkEntity, Command, bool> onExecuteCommand;
//     public event Func<NetworkEntity, Command, bool> onLocalAndRemoteResultEqual;
//
//     private BoltEntity _boltEntity;
//     
//     void Awake()
//     {
//         _boltEntity = GetComponent<BoltEntity>();
//     }
//     
//     public T GetState<T>() where T : IState
//     {
//         return (T)state;
//     }
//     
//     public override void Attached()
//     {
//         base.Attached();
//         
//         if(onAttached != null)
//         {
//             onAttached(this);
//         }
//     }
//
//     public override void Detached()
//     {
//         base.Detached();
//         
//         if(onDetached != null)
//         {
//             onDetached(this);
//         }
//     }
//
//     public override void SimulateOwner()
//     {
//         base.SimulateOwner();
//
//         if(onSimulateOwner != null)
//         {
//             onSimulateOwner(this);
//         }
//     }
//
//     public override void SimulateController()
//     {
//         base.SimulateController();
//
//         if(onSimulateController != null)
//         {
//             onSimulateController(this);
//         }
//     }
//
//     public override void ControlGained()
//     {
//         base.ControlGained();
//         
//         if(onControlGained != null)
//         {
//             onControlGained(this);
//         }
//     }
//
//     public override void ControlLost()
//     {
//         base.ControlLost();
//         
//         if(onControlLost != null)
//         {
//             onControlLost(this);
//         }
//     }
//
//     public override void MissingCommand(Command previous)
//     {
//         base.MissingCommand(previous);
//
//         if(onMissingCommand != null)
//         {
//             onMissingCommand(this, previous);
//         }
//     }
//
//     public override void ExecuteCommand(Command command, bool resetState)
//     {
//         base.ExecuteCommand(command, resetState);
//
//         if(onExecuteCommand != null)
//         {
//             onExecuteCommand(this, command, resetState);
//         }
//     }
//     
//     
//     public override bool LocalAndRemoteResultEqual(Command command)
//     {
//         bool result = false;
//         
//         if(onLocalAndRemoteResultEqual != null)
//         {
//             result = onLocalAndRemoteResultEqual(this, command);
//         }
//         
//         return result;
//     }
// }
