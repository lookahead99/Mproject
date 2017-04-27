
--print('hello world2222')
local GameManager = {}
gameStateManager = require'test.GameStatemanager'
 function  GameManager:Init()
        --print("call gameManager init")
        gameStateManager:Init()
end

function  GameManager:Update()
         --print("call gameManager Update")
         gameStateManager:Update()
end
return GameManager
