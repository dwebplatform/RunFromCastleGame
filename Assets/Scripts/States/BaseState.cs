using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseState 
{       
        private string _name;
        protected Text textObj;
        public BaseState(string name){
            _name = name;
            textObj = GameObject.Find("Canvas/Text").GetComponent<Text>();
        }

        public virtual void Enter(){
            Debug.Log(_name);
            textObj.text = _name;
            
        }
        public virtual void HandleInput(){}
        public virtual  void  LogicUpdate(){}
        public virtual void PhysicsUpdate(){}
        public virtual void Exit(){}
}
