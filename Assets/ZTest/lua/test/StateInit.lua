local StateInit = {}

local gameStatemanager = gameStateManager
 function StateInit:Enter()
    print("Enter State Init")
    gameStatemanager:ChangeState(GS_CHECKVERSION,false)
end

function StateInit:Exit()
    print("Exit State Init")
end

return StateInit