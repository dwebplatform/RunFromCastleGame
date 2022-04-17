using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState 
{       
        private string _name;
        public BaseState(string name){
            _name = name;
        }

        public virtual void Enter(){
            Debug.Log(_name);
        }
        public virtual void HandleInput(){}
        public virtual  void  LogicUpdate(){}
        public virtual void PhysicsUpdate(){}
        public virtual void Exit(){}
}
