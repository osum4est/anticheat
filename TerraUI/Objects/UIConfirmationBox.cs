using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TerraUI.Objects;
using TerraUI.Panels;

namespace SimpleTools.TerraUI.Objects
{
    class UIConfirmationBox : UIObject
    {
        private static float width = 500;
        private static float height = 150;

        private static float buttonWidth = 100;
        private static float buttonHeight = 25;

        private static float spacing = 25;

        private UIPanel Panel;
        private UILabel Message;
        private UIButton ButtonYes;
        private UIButton ButtonNo;

        public event Action OnYesClick;
        public event Action OnNoClick;

        public UIConfirmationBox(string message, UIObject parent = null, bool allowFocus = false, bool acceptsKeyboardInput = false) : base(new Vector2(Main.screenWidth/2f - width/2f, Main.screenHeight/2f - height/2f), new Vector2(width, height), parent, allowFocus, acceptsKeyboardInput)
        {
            Panel = new UIPanel(Position, Size);

            Message = new UILabel(new Vector2(spacing, spacing), new Vector2(width - spacing*2, height - spacing*2), message, Main.fontMouseText, parent: Panel);

            ButtonYes = new UIButton(new Vector2(width - spacing - buttonWidth, height - spacing - buttonHeight), new Vector2(buttonWidth, buttonHeight), Main.fontItemStack, "Yes", parent: Panel);
            ButtonNo = new UIButton(new Vector2(width - spacing*2 - buttonWidth*2, height - spacing - buttonHeight), new Vector2(buttonWidth, buttonHeight), Main.fontItemStack, "No", parent: Panel);

            ButtonYes.MouseDown += (a, b) => OnYesClick();
            ButtonNo.MouseDown += (a, b) => OnNoClick();
            
            Panel.Children.Add(Message);
            Panel.Children.Add(ButtonYes);
            Panel.Children.Add(ButtonNo);
        }

        public override void Update()
        {
            Panel.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Panel.Draw(spriteBatch);
        }
    }
}
