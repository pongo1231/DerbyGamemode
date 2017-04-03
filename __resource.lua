files {
	"html/ui.html",
	"html/ui.css",
	"html/ui.js",
	"html/Roboto-Bold.ttf",
}

ui_page "html/ui.html"

server_script "server.lua"
client_scripts {
	"Derby.net.dll",
	"nui.lua"
}

-- Object-Loader stuff

local function object_entry(data)
	dependency 'object-loader'

	files(data)
	object_file(data)
end

object_entry 'derbymap.xml'