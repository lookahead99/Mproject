function start()
	print("lua start...")
end

function update()
	local r = CS.UnityEngine.Vector3.up * CS.UnityEngine.Time.deltaTime * speed
	self.transform:Rotate(r)
end

function ondestroy()
    print("lua destroy")
end