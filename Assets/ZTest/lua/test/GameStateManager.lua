   --  GameStateManager --- ¿ØÖÆgameState --
   
 GS_INIT = 1
 GS_CHECKVERSION= 2
 GS_LOADING = 3

  GameStateManager = 
 {
   curState = nil,
   nextState = nil,
   allStates = {},
}
 function GameStateManager:Init()
   -- print("call gameManager init")
    self.allStates[GS_INIT] = require'test.StateInit'
    self.allStates[GS_CHECKVERSION] = require'test.StateCheckVersion'
    self:ChangeState(GS_INIT,true)
end
 function GameStateManager:ChangeState(state,now)
     --print("call Change state "..tostring(state).." "..tostring(now) )
   if (now)   then
        self:ChangeStateNow(state)
   else
        self.nextState = state
         --print("set Next state "..tostring(state))
    end
end
 function GameStateManager:Update()
   -- print("GameStateManager Update state "..tostring(self.nextState))
    if (self.nextState ~= nil)
    then
        self:ChangeStateNow(self.nextState)
        self.nextState = nil
    end
end
 function GameStateManager:ChangeStateNow(state)
    --print("call Change state Now"..tostring(state) )
    if (self.curState ~= nil) then
        self.curState:Exit()
    end
    self.curState = self.allStates[state]
    self.curState:Enter()
end
return GameStateManager