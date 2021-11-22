using UnityEngine;

namespace Game.SweetsWar
{
    public class GameConstants
    {
        public const bool DebugMode = false;

        // Game Settings
        public const byte MAX_PLAYERS_PER_ROOM = 4;
        public const string FIXED_REGION_ASIA = "asia";

        // From InputManager (Edit > ProjectSettings > InputManager)
        public const string VERTICAL = "Vertical";
        public const string HORIZONTAL = "Horizontal";
        public const string MOUSE_Y = "Mouse Y";
        public const string MOUSE_X = "Mouse X";
        public const string JOYSTICK_LOOK_Y = "Look Y";
        public const string JOYSTICK_LOOK_x = "Look X";
        public const string BUTTON_JUMP = "Jump";
        public const string BUTTON_FIRE = "Fire";
        public const string BUTTON_GAMEPAD_FIRE = "Gamepad Fire";
        public const string BUTTON_SPRINT = "Sprint";
        public const string BUTTON_CROUCH = "Crouch";
        public const string BUTTON_AIM = "Aim";
        public const string BUTTON_GAMEPAD_AIM = "Gamepad Aim";
        public const string BUTTON_SWITCH_WEAPON = "Mouse ScrollWheel";
        public const string BUTTON_GAMEPAD_SWITCH_WEAPON = "Gamepad Switch";
        public const string BUTTON_NEXT_WEAPON = "NextWeapon";
        public const string BUTTON_PAUSE_MENU = "Pause Menu";
        public const string BUTTON_SUBMIT = "Submit";
        public const string BUTTON_CANCEL = "Cancel";

        // Scene
        public const string SCENE_TITLE = "TitleScene";
        public const string SCENE_LOBBY = "LobbyScene";
        public const string SCENE_READY = "ReadyScene";
        public const string SCENE_GAME = "GameScene";
        public const string SCENE_GAME_TEAM = "GameSceneTeam";
        public const string SCENE_END = "EndScene";

        // KEY
        public const string GAME_MODE = "GAME_MODE";
        public const string GAME_MODE_PERSONAL_BATTLE = "個人戰";
        public const string GAME_MODE_TEAM_FIGHT = "團戰";
        public const string PLAYER_NAME_PREFAB_KEY = "PlayerName";
        public const string TAG_PLAYER = "Player";
        public const string TAG_ITEM = "Item";

        // Props
        public const string IS_PLAYER_READY = "IS_PLAYER_READY";
        public const string PLAYER_LOADED_LEVEL = "PLAYER_LOADED_LEVEL";
        public const string K_PROP_PLAYER_INDEX = "K_PROP_PLAYER_INDEX";

        // UI
        public const string COLOR_YELLOW = "FDBB08";

        // Method
        public static string GetSceneByGameMode(string gameMode)
        {
            switch (gameMode)
            {
                case GAME_MODE_TEAM_FIGHT:
                    return SCENE_GAME_TEAM;
            }
            return SCENE_GAME;
        }

        public static Color GetModeColor(string mode)
        {
            switch (mode)
            {
                case GAME_MODE_TEAM_FIGHT:
                    return Color.red;
            }

            return Color.black;

        }
        public static Color GetColor(int number)
        {
            switch (number)
            {
                case 0: return Color.red;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.white;
            }

            return Color.black;
        }

    }
}

