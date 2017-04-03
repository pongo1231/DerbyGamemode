local spectatepos = {x = -2535.302, y = -1673.258, z = 15.3088}
local spawns = {
	{x = -2601.64086914063, y = -1565.70922851563, z = 5.00233268737793, heading = 182.016693115234},
	{x = -2615.0947265625, y = -1576.59899902344, z = 5.00553226470947, heading = 267.943389892578},
	{x = -2588.787109375, y = -1641.69494628906, z = 7.45322370529175, heading = 356.982330322266},
	{x = -2530.73071289063, y = -1606.7783203125, z = 7.90370464324951, heading = 85.7027969360352},
	{x = -2538.12963867188, y = -1646.87219238281, z = 9.27953910827637, heading = 223.144149780273},
	{x = -2540.5400390625, y = -1649.6083984375, z = 9.20371627807617, heading = 224.100769042969},
	{x = -2541.81323242188, y = -1676.09313964844, z = 8.81536102294922, heading = 311.059753417969},
	{x = -2539.06982421875, y = -1678.53674316406, z = 8.81545352935791, heading = 310.707824707031},
	{x = -2515.97119140625, y = -1681.09289550781, z = 8.81360054016113, heading = 42.3859481811523},
	{x = -2505.82055664063, y = -1674.45861816406, z = 8.78554344177246, heading = 45.0059432983398},
	{x = -2510.38989257813, y = -1647.97802734375, z = 8.78160858154297, heading = 130.31591796875},
	{x = -2507.76342773438, y = -1650.47497558594, z = 8.78167343139648, heading = 131.987197875977},
	{x = -2448.70190429688, y = -1676.36242675781, z = 14.7131519317627, heading = 65.7629928588867},
	{x = -2450.37353515625, y = -1664.89331054688, z = 14.7128934860229, heading = 127.599166870117},
	{x = -2462.92114257813, y = -1658.61291503906, z = 14.7131576538086, heading = 194.268463134766},
	{x = -2472.30639648438, y = -1665.81848144531, z = 14.7132205963135, heading = 249.605911254883},
	{x = -2469.27294921875, y = -1678.80737304688, z = 14.7130222320557, heading = 313.294494628906},
	{x = -2459.62670898438, y = -1682.32678222656, z = 14.7131519317627, heading = 0.737317264080048},
	{x = -2484.01391601563, y = -1666.62353515625, z = 14.7054443359375, heading = 82.7607727050781},
	{x = -2511.08666992188, y = -1719.42138671875, z = 14.5260286331177, heading = 133.021987915039},
	{x = -2510.138671875, y = -1735.11364746094, z = 14.468864440918, heading = 49.4168548583984},
	{x = -2529.13818359375, y = -1733.34265136719, z = 14.4691257476807, heading = 298.276641845703},
	{x = -2522.4296875, y = -1718.89562988281, z = 14.4690685272217, heading = 189.190826416016},
	{x = -2523.6923828125, y = -1703.86560058594, z = 14.4457597732544, heading = 6.50497961044312},
}
local players = {}
local gamerunning = false

RegisterServerEvent("derby:newplayer")
AddEventHandler("derby:newplayer", function(source)
	table.insert(players, {source = source})
	if tableLength(players) == 2 then
		newGame()
	end
end)

AddEventHandler("playerDropped", function(reason)
	removePlayer(source)
end)

RegisterServerEvent("derby:playerout")
AddEventHandler("derby:playerout", function(source)
	TriggerClientEvent("chatMessage", -1, "DERBY", {255, 0, 0}, "^1" .. GetPlayerName(source) .. " is out!")
	setPlayerOut(source)

	local playersleft = getIngamePlayers()
	if tableLength(playersleft) <= 1 then
		if tableLength(playersleft) == 1 then
			TriggerClientEvent("chatMessage", -1, "DERBY", {255, 0, 0}, "^2" .. GetPlayerName(playersleft[1]) .. " won!")
		else
			TriggerClientEvent("chatMessage", -1, "DERBY", {255, 0, 0}, "^3It's a tie!")
		end

		gamerunning = false
		newGame()
	end
end)

function removePlayer(source)
	local index
	for n, i in pairs(players) do
		if i.source == source then
			index = n
		end
	end

	if n then
		table.remove(players, n)
	end
end

function setPlayerOut(source)
	for n, i in pairs(players) do
		if i.source == source then
			i.out = true
		end
	end
end

function getIngamePlayers()
	local t = {}
	for _, i in pairs(players) do
		if i.ingame and not i.out then
			table.insert(t, i.source)
		end
	end

	return t
end

function newGame()
	if gamerunning then
		return
	end

	gamerunning = true
	resetSpawns()
	for n, i in pairs(players) do
		i.ingame = true
		i.out = false

		local playerSpawn = getUnusedSpawn()
		if playerSpawn then
			TriggerClientEvent("derby:setspawn", i.source, playerSpawn.x, playerSpawn.y, playerSpawn.z, playerSpawn.heading,
				spectatepos.x, spectatepos.y, spectatepos.z)
		end
	end

	SetTimeout(200, function()
		TriggerClientEvent("derby:startgame", -1)
	end)
end

function tableLength(T)
	local count = 0
	for _, i in pairs(T) do count = count + 1 end
	return count
end

function resetSpawns()
	for n, i in pairs(spawns) do
		i.used = false
	end
end

function getUnusedSpawn()
	local tries = 30
	while tries > 0 do
		local spawn = spawns[math.random(1, tableLength(spawns))]
		if not spawn.used then
			spawn.used = true
			return spawn
		end
		tries = tries - 1
	end

	return false
end

SetTimeout(5000, function()
	newGame()
end)