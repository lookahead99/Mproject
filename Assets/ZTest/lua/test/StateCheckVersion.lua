local StateCheckVersion = {}

--local gameStatemanager = require'test.GameStateManager'
 function StateCheckVersion:Enter()
    print("Enter State checkVerison")
    --gameStatemanager.ChangeState(GS_CHECKVERSION)
end

 function StateCheckVersion:Exit()
    print("Exit State checkVerison")
end

return StateCheckVersion