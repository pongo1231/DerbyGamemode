using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Derby
{
    class Util : BaseScript
    {
        public static void SendLocalMessage(string msg)
        {
            TriggerEvent("chatMessage", "DERBY", new int[] { 255, 0, 0 }, msg);
        }

        public static void SetVehNumPlate(Vehicle veh, string text)
        {
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, veh.Handle, text);
        }

        public static void GetPlayerRGBColor(out int r, out int g, out int b)
        {
            OutputArgument outR = new OutputArgument();
            OutputArgument outG = new OutputArgument();
            OutputArgument outB = new OutputArgument();
            Function.Call(Hash.GET_PLAYER_RGB_COLOUR, Game.Player.Handle, outR, outG, outB);
            r = outR.GetResult<int>();
            g = outG.GetResult<int>();
            b = outB.GetResult<int>();
        }

        public static void SetVehRGBColor(Vehicle veh, int r, int g, int b)
        {
            Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR, veh.Handle, r, g, b);
            Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, veh.Handle, r, g, b);
        }
    }
}
