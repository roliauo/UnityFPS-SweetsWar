using UnityEngine;

namespace Game.SweetsWar
{
    public class GameConstants
    {
        public const bool DebugMode = false;

        // Game Settings
        public const byte MAX_PLAYERS_PER_ROOM = 4;
        public const byte MAX_TEAMS = 2;
        public const byte PLAYERS_PER_TEAM = 2;
        public const string FIXED_REGION_ASIA = "asia";
        public const byte INPUT_TEXT_LIMIT = 20;

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
        public const string GAME_MODE_SOLO = "個人戰";
        public const string GAME_MODE_TEAM = "團戰";
        public const string PLAYER_NAME_PREFAB_KEY = "PlayerName";
        public const string TAG_PLAYER = "Player";
        public const string TAG_ITEM = "Item";

        // Props
        public const string IS_PLAYER_READY = "IS_PLAYER_READY";
        public const string PLAYER_LOADED_LEVEL = "PLAYER_LOADED_LEVEL";
        //public const string K_PROP_PLAYER_COLOR = "K_PROP_PLAYER_COLOR";
        public const string K_PROP_WEAPON_VIEW_ID = "weaponViewID";
        public const string K_PROP_HEALTH = "health";
        public const string K_PROP_MAX_HEALTH = "maxHealth";
        public const string K_PROP_IS_DEAD = "isDead";
        public const string K_PROP_KILLS = "kills";
        public const string K_PROP_DAMAGE_POINTS = "damagePoints";
        public const string K_PROP_CRAFT_NUMBER = "craftNumber";
        public const string K_PROP_SCORE = "score";
        public const string K_PROP_WINNER = "winner";
        public const string K_PROP_TEAM = "team";

        // UI
        public const string COLOR_YELLOW = "FDBB08";

        // ID Range
        public const int TREASURE_ID_MIN = 1000;
        public const int TREASURE_ID_MAX = 1002;

        // Item Type
        public const string ITEM_TYPE_ITEM = "item";
        public const string ITEM_TYPE_WEAPON = "weapon";
        public const string ITEM_TYPE_TREASURE = "treasure";

        // Method
        public static string GetSceneByGameMode(string gameMode)
        {
            switch (gameMode)
            {
                case GAME_MODE_TEAM:
                    return SCENE_GAME_TEAM;
            }
            return SCENE_GAME;
        }

        public static Color GetModeColor(string mode)
        {
            switch (mode)
            {
                case GAME_MODE_TEAM:
                    return Color.red;
            }

            return Color.black;

        }
        public static Color GetColor(int number)
        {
            switch (number)
            {
                case 0: return new Color32(241, 126, 126, 255); //Color.red;
                case 1: return new Color32(100, 100, 255, 255); //Color.blue;
                case 2: return new Color32(245, 243, 135, 255); //Color.yellow;
                case 3: return new Color32(55, 166, 55, 255); //Color.green;    
                default: return Color.red;
            }

            return Color.white;
        }

    }
}

