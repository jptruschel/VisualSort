using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;

namespace VisualSort
{
    public class GUI
    {
        // Definições de texturas usadas pela GUI
        public static Texture2D[] ArrowBtns;
        public static Texture2D Arrow2;
        public static Texture2D wBox;
        public static Texture2D stLine;
        public static Texture2D CheckTex;
        public static SpriteFont[] Fonts;
        public static List<TGUIObject> Components;

        public GUI()
        {
            Components = new List<TGUIObject>();
        }
        public TGUIPanel NewPanel(Vector2 Pos, Vector2 Size)
        {
            Components.Add(new TGUIPanel(Pos, Size));
            return (Components[Components.Count - 1] as TGUIPanel);
        }

        // Classes usadas na GUI
        public abstract class TGUIObject
        {
            public Vector2 Pos;
            public Vector2 Size;
            public string Text;
            public Color TextColor;
            public bool AutoSize;
            public int Font;
            public TGUIObject Parent;
            public bool Visible, Enabled;
            public int BorderSize;
            // Creator
            public TGUIObject(TGUIObject Parent)
            {
                this.Parent = Parent;
                Pos = new Vector2(0, 0);
                Size = new Vector2(1, 1);
                Text = "";
                TextColor = Color.White;
                Font = 1;
                AutoSize = false;
                BorderSize = 1;
                Visible = true;
                Enabled = true;
            }
            protected TGUIObject(Vector2 Pos, Vector2 Size)
            {
                this.Parent = null;
                this.Pos = Pos;
                this.Size = Size;
                Text = "";
                TextColor = Color.White;
                AutoSize = false;
                Font = 1;
                BorderSize = 1;
                Visible = true;
            }
            public virtual void Draw(SpriteBatch spriteBatch)
            {
                // Draw 4 1xSize lines
                if ((BorderSize > 0) && (Parent != null))
                {
                    // |c
                    spriteBatch.Draw(wBox,
                        new Rectangle((int)Pos.X - BorderSize + (int)Parent.Pos.X, (int)Pos.Y + (int)Parent.Pos.Y, (int)BorderSize, (int)Size.Y),
                        null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    // c|
                    spriteBatch.Draw(wBox,
                        new Rectangle((int)Pos.X + (int)Size.X + (int)Parent.Pos.X, (int)Pos.Y + (int)Parent.Pos.Y, (int)BorderSize, (int)Size.Y),
                        null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    // -
                    spriteBatch.Draw(wBox,
                        new Rectangle((int)Pos.X + (int)Parent.Pos.X, (int)Pos.Y - BorderSize + (int)Parent.Pos.Y, (int)Size.X, (int)BorderSize),
                        null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    // _
                    spriteBatch.Draw(wBox,
                        new Rectangle((int)Pos.X + (int)Parent.Pos.X, (int)Pos.Y + (int)Size.Y + (int)Parent.Pos.Y, (int)Size.X, (int)BorderSize),
                        null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }
            }
            public virtual void Update(MouseState mouseState)
            {
                if (AutoSize)
                    Size = Fonts[Font].MeasureString(Text) + new Vector2(2, 1);
            }
        }
        // Um Panel (um tipo de Container para outros componentes)
        public class TGUIPanel : GUI.TGUIObject
        {
            public List<TGUIObject> Components;
            public Color color;
            public float backgroundAlpha;
            public bool MouseOver;
            public bool Hidden;

            private Vector2 Speed, FinalPos, hideOffset;
            private float SpeedAlpha;
            private byte AnimState;
            private MouseState oldMouseState;
            // Creator
            public TGUIPanel(Vector2 Pos, Vector2 Size)
                : base(Pos, Size)
            {
                Components = new List<TGUIObject>();
                color = Color.DarkSlateGray;
                backgroundAlpha = 0.92f;
                Hidden = false;
                Visible = true;
                Enabled = true;
            }
            private bool Moving()
            {
                return ((Math.Abs(Speed.X) > 0.01f) || (Math.Abs(Speed.Y) > 0.01f));
            }
            // Update
            public override void Update(MouseState mouseState)
            {
                base.Update(mouseState);
                bool movingstopped = true;

                if ((Moving()) && (Hidden || (AnimState == 1) || (AnimState == 3)))
                {
                    movingstopped = false;
                    Pos += Speed;
                    Speed = new Vector2((FinalPos.X - Pos.X) / SpeedAlpha, (FinalPos.Y - Pos.Y) / SpeedAlpha);
                }
                if ((movingstopped) && (Hidden))
                {
                    if (AnimState == 1)
                    {
                        AnimState = 2;
                    }
                    if (AnimState == 3)
                    {
                        Hidden = false;
                        AnimState = 0;
                    }
                }

                if (!Rectangle.Intersect(
                    new Rectangle(
                        (int)(this.Pos.X),
                        (int)(this.Pos.Y),
                        (int)(Size.X),
                        (int)(Size.Y)),
                     new Rectangle(
                         (int)mouseState.X,
                         (int)mouseState.Y,
                         1, 1
                         )).IsEmpty)
                {
                    MouseOver = true;
                    if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        && (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) && (Hidden) && (!Moving()))
                        Show(8f);
                }
                else
                {
                    MouseOver = false;
                    if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        if (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                            if (this == Renderer.FilterPanel)
                                if (Visible)
                                    if (!Renderer.FilterButton.MouseOver)
                                        Renderer.ToggleFilterPanel(0);
                }
                oldMouseState = mouseState;
                if (Visible && Enabled)
                    foreach (TGUIObject Component in Components)
                    {
                        Component.Update(mouseState);
                    }
            }
            // Draw
            public override void Draw(SpriteBatch spriteBatch)
            {
                if (Visible)
                {
                    base.Draw(spriteBatch);
                    if (MouseOver)
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)(Pos.X),
                                (int)(Pos.Y),
                                (int)Size.X,
                                (int)Size.Y),
                            null, color * (Math.Min(backgroundAlpha * 2, 0.98f)), 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    else
                        spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)(Pos.X),
                            (int)(Pos.Y),
                            (int)Size.X,
                            (int)Size.Y),
                        null, color * backgroundAlpha, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    if (Text != null)
                        spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos, TextColor);

                    // Draw all components
                    foreach (TGUIObject Component in Components)
                    {
                        if ((Component.Visible) && ((!Hidden) || (AnimState == 3) || (AnimState == 1)))
                            Component.Draw(spriteBatch);
                    }
                }
            }
            // Add a component to be drawned
            public TGUIObject AddComponent(TGUIObject Component)
            {
                Components.Add(Component);
                Component.Parent = this;
                if (Component is TGUIImgBtn)
                    (Component as TGUIImgBtn).Tag = Components.Count - 1;
                return Components[Components.Count - 1];
            }
            // Hide the panel
            public void Hide(Vector2 HideOffset, float Alpha)
            {
                if (AnimState == 0)
                {
                    Hidden = true;
                    Speed = new Vector2(HideOffset.X / Alpha, HideOffset.Y / Alpha);
                    SpeedAlpha = Alpha;
                    FinalPos = Pos + HideOffset;
                    hideOffset = HideOffset;
                    AnimState = 1;
                }
            }
            // Show the panel
            public void Show(float Alpha)
            {
                if (AnimState ==2)
                {
                    Speed = new Vector2(-hideOffset.X / Alpha, -hideOffset.Y / Alpha);
                    SpeedAlpha = Alpha;
                    this.FinalPos = Pos - hideOffset;
                    hideOffset = -hideOffset;
                    AnimState = 3;
                }
            }
        }
        // Um Botão com Imagem, caso queira
        public class TGUIImgBtn : GUI.TGUIObject
        {
            public Texture2D Image, HotImage, ClickImage;
            public Color ImageColor;
            public bool DrawImageResized;
            public bool useImage, useHotImage, useClickImage;
            public bool MouseOver, Clicking, Highlighted;
            public Func<int,bool> OnClick;
            public Func<int,bool> OnRelease;
            public Func<int,bool> OnMouseEnter;
            public Func<int,bool> OnMouseLeave;
            private MouseState oldMouseState;
            public Color backgroundColor;
            public SpriteEffects spriteEffect;
            public int Tag;
            public TGUIImgBtn(Vector2 Pos, Vector2 Size, string Text)
                : base(Pos, Size)
            {
                this.Text = Text;
                MouseOver = false;
                TextColor = Color.Black;
                BorderSize = 0;
                AutoSize = true;
                useImage = false;
                useHotImage = false;
                Highlighted = false;
                useClickImage = false;
                backgroundColor = Color.Azure;
                ImageColor = Color.White;
                spriteEffect = SpriteEffects.None;
                Visible = true;
                Enabled = true;
            }
            // Verifica clique do mouse no componente
            public override void Update(MouseState mouseState)
            {
                base.Update(mouseState);

                if (!Rectangle.Intersect(
                    new Rectangle(
                        (int)(this.Pos.X + Parent.Pos.X),
                        (int)(this.Pos.Y + Parent.Pos.Y),
                        (int)(Size.X),
                        (int)(Size.Y)),
                     new Rectangle(
                         (int)mouseState.X,
                         (int)mouseState.Y,
                         1, 1
                         )).IsEmpty)
                {
                    if (!MouseOver)
                        if (OnMouseEnter != null)
                            OnMouseEnter(Tag);
                    MouseOver = true;
                    if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                        (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))
                    {
                        Clicking = true;
                        if ((OnClick != null) && Enabled)
                            OnClick(Tag);
                    }
                }
                else
                {
                    if (MouseOver && Enabled)
                        if (OnMouseLeave != null)
                            OnMouseLeave(Tag);
                    MouseOver = false;
                }
                if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    Clicking = false;
                    if (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        if ((OnRelease != null) && Enabled)
                            OnRelease(Tag);
                    }
                }
                oldMouseState = mouseState;
                MouseOver = MouseOver && Enabled;
                Clicking = Clicking && Enabled;
            }
            // Desenha
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                float Multiplier = 0.76f;
                if (MouseOver || Highlighted)
                    Multiplier = 0.92f;
                if (Clicking)
                    Multiplier = 1.0f;
                // Box
                spriteBatch.Draw(wBox,
                new Rectangle(
                    (int)Pos.X + (int)Parent.Pos.X,
                    (int)Pos.Y + (int)Parent.Pos.Y,
                    (int)Size.X,
                    (int)Size.Y),
                null, backgroundColor * Multiplier, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                if (Enabled)
                    spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos + Parent.Pos, TextColor);
                else
                    spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos + Parent.Pos, Color.Gray);

                if (useImage)
                {
                    if ((useHotImage) && (MouseOver || Highlighted))
                    {
                        if (DrawImageResized)
                            spriteBatch.Draw(HotImage,
                                new Rectangle(
                                    (int)Pos.X + (int)Parent.Pos.X,
                                    (int)Pos.Y + (int)Parent.Pos.Y,
                                    (int)Size.X,
                                    (int)Size.Y),
                                null, ImageColor, 0f, Vector2.Zero, spriteEffect, 0f);
                        else
                            spriteBatch.Draw(HotImage,
                                new Rectangle(
                                    (int)Pos.X + (int)Parent.Pos.X,
                                    (int)Pos.Y + (int)Parent.Pos.Y,
                                    (int)HotImage.Width,
                                    (int)HotImage.Height),
                                null, ImageColor, 0f, Vector2.Zero, spriteEffect, 0f);
                    } else
                        if ((useClickImage) && (Clicking))
                        {
                            if (DrawImageResized)
                                spriteBatch.Draw(ClickImage,
                                    new Rectangle(
                                        (int)Pos.X + (int)Parent.Pos.X,
                                        (int)Pos.Y + (int)Parent.Pos.Y,
                                        (int)Size.X,
                                        (int)Size.Y),
                                    null, ImageColor, 0f, Vector2.Zero, spriteEffect, 0f);
                            else
                                spriteBatch.Draw(ClickImage,
                                    new Rectangle(
                                        (int)Pos.X + (int)Parent.Pos.X,
                                        (int)Pos.Y + (int)Parent.Pos.Y,
                                        (int)ClickImage.Width,
                                        (int)ClickImage.Height),
                                    null, ImageColor, 0f, Vector2.Zero, spriteEffect, 0f);
                        }
                        else
                            if (DrawImageResized)
                                spriteBatch.Draw(Image,
                                    new Rectangle(
                                        (int)Pos.X + (int)Parent.Pos.X,
                                        (int)Pos.Y + (int)Parent.Pos.Y,
                                        (int)Size.X,
                                        (int)Size.Y),
                                    null, ImageColor, 0f, Vector2.Zero, spriteEffect, 0f);
                            else
                                spriteBatch.Draw(Image,
                                    new Rectangle(
                                        (int)Pos.X + (int)Parent.Pos.X,
                                        (int)Pos.Y + (int)Parent.Pos.Y,
                                        (int)Image.Width,
                                        (int)Image.Height),
                                    null, ImageColor, 0f, Vector2.Zero, spriteEffect, 0f);
                }
            }
        }
        // Um Label
        public class TGUILabel : GUI.TGUIObject
        {
            public Color backgroundColor;
            public bool transparentBackground;
            public TGUILabel(Vector2 Pos, Vector2 Size, string Text)
                : base(Pos, Size)
            {
                this.Text = Text;
                TextColor = Color.Black;
                AutoSize = true;
                transparentBackground = true;
                backgroundColor = Color.White;
                BorderSize = 0;
                Visible = true;
                Enabled = true;
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                if (!transparentBackground)
                    spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X,
                            (int)Pos.Y + (int)Parent.Pos.Y,
                            (int)Size.X,
                            (int)Size.Y),
                        null, backgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                if (Enabled)
                    spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos + Parent.Pos, TextColor);
                else
                    spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos + Parent.Pos, Color.Gray);
            }
        }
        // Uma lista
        public class TGUIList : GUI.TGUIObject
        {
            public List<string> Items;
            public int ItemIndex;
            public int Offset;
            private int ionView;
            private int yPerLine;
            public Func<int, bool> OnClick;
            public Color color, verticalBarColor;
            public int VerticalBarWidth;
            private float VerticalBarHeight;
            public bool MouseOverList, MouseOverBar, Clicking;
            public bool ResizeOnMouse;
            private MouseState oldMouseState;
            private Vector2 FirstClickPos;
            private float prevWheelValue, currWheelValue;
            public float xOffsetTextSize;
            public TGUIList(Vector2 Pos, Vector2 Size)
                : base(Pos, Size)
            {
                Font = 2;
                Items = new List<string>();
                Offset = 0;
                yPerLine = -1;
                color = Color.White;
                TextColor = Color.Black;
                verticalBarColor = Color.SlateGray;
                VerticalBarWidth = 16;
                currWheelValue = 0f;
                prevWheelValue = 0f;
                Visible = true;
                Enabled = true;
                ItemIndex = -1;
                xOffsetTextSize = 0f;
                ResizeOnMouse = true;
                //UpDownSize = new Vector2(16, 16);
            }
            public override void Update(MouseState mouseState)
            {
                base.Update(mouseState);
                if (yPerLine == -1)
                {
                    yPerLine = (int)(Fonts[Font].MeasureString("aAjgy1234567890").Y);
                    ionView = (int)(Size.Y / yPerLine);
                }
                VerticalBarHeight = (int)(Math.Pow((Size.Y), 2) / (ionView * Items.Count));
                if (VerticalBarHeight > Size.Y)
                    VerticalBarHeight = Size.Y - 2;

                prevWheelValue = currWheelValue;
                currWheelValue = mouseState.ScrollWheelValue;

                if (Enabled)
                {
                    if (!Rectangle.Intersect(
                        new Rectangle(
                            (int)(this.Pos.X + Parent.Pos.X),
                            (int)(this.Pos.Y + Parent.Pos.Y - 5),
                            (int)(Size.X + xOffsetTextSize + 10),
                            (int)(Size.Y)),
                         new Rectangle(
                             (int)mouseState.X,
                             (int)mouseState.Y,
                             1, 1
                             )).IsEmpty)
                    {
                        xOffsetTextSize = 0;
                        if (ResizeOnMouse)
                        {
                            foreach (string s in Items)
                            {
                                float ns = Fonts[Font].MeasureString(s).X;
                                if (ns > xOffsetTextSize)
                                    xOffsetTextSize = ns;
                            }
                            xOffsetTextSize = Math.Min(xOffsetTextSize, Size.X * 2);//AppGraphics.ScreenCenter.X);
                            if (xOffsetTextSize < Size.X)
                                xOffsetTextSize = 0;
                            else
                                xOffsetTextSize -= (Size.X - VerticalBarWidth - 5);
                        }

                        if (Rectangle.Intersect(
                        new Rectangle(
                                    (int)(Pos.X + Parent.Pos.X + xOffsetTextSize + Size.X - VerticalBarWidth - 1),
                                    (int)((Pos.Y + Parent.Pos.Y + 1)) - 5,
                                    (int)VerticalBarWidth,
                                    (int)Size.Y),
                         new Rectangle(
                             (int)mouseState.X,
                             (int)mouseState.Y,
                             1, 1
                             )).IsEmpty)
                        {
                            MouseOverList = true;
                            MouseOverBar = false;
                            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                                if (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                                {
                                    int i = 0;
                                    ItemIndex = Offset;
                                    while (i <= Math.Min(ionView, Items.Count - Offset))
                                    {
                                        if (mouseState.Y - Pos.Y + Parent.Pos.Y < yPerLine * i)
                                            i = Math.Max(ionView, Items.Count - Offset) + 10;
                                        i++;
                                        ItemIndex++;
                                    }
                                    ItemIndex -= 2;
                                    if (OnClick != null)
                                        OnClick(ItemIndex);
                                }
                        }
                        else
                        {
                            if ((Rectangle.Intersect(
                                new Rectangle(
                                            (int)(Pos.X + Parent.Pos.X + xOffsetTextSize + Size.X - VerticalBarWidth - 1),
                                            (int)((Pos.Y + Parent.Pos.Y ))-55,
                                            (int)VerticalBarWidth,
                                            (int)VerticalBarHeight+10),
                                 new Rectangle(
                                     (int)mouseState.X,
                                     (int)mouseState.Y,
                                     1, 1
                                     )).IsEmpty)
                                && (((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                                (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))))
                            {
                                Offset = (int)
                                    -((Pos.Y + Parent.Pos.Y - mouseState.Y) / ((Size.Y - 2) / Items.Count));
                            }
                            MouseOverBar = true;
                            MouseOverList = false;
                        }
                        if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                            (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) &&
                            (MouseOverBar))
                        {
                            Clicking = true;
                            FirstClickPos = new Vector2(mouseState.X, mouseState.Y);
                        }
                        Offset += (int)((prevWheelValue - currWheelValue) * 0.05);
                    }
                    else
                    {
                        if (Clicking == false)
                        {
                            xOffsetTextSize = 0;
                            MouseOverBar = false;
                            MouseOverList = false;
                        }
                    }

                    if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        Clicking = false;
                    }

                    if ((Clicking) && (MouseOverBar || MouseOverList))
                    {
                        double Dif = Math.Abs((FirstClickPos.Y - mouseState.Y)) / ((Size.Y - VerticalBarHeight) / (Items.Count - ionView));
                        if (Dif>1)
                        {
                            if ((FirstClickPos.Y - mouseState.Y) > 0)
                            {
                                while(Math.Floor(Dif)>1)
                                {
                                    Offset--;
                                    FirstClickPos.Y -= (Size.Y - VerticalBarHeight) / (Items.Count - ionView);
                                    Dif--;
                                }
                            }
                            else
                            {
                                 while(Math.Floor(Dif)>1)
                                {
                                    Offset++;
                                    FirstClickPos.Y += (Size.Y - VerticalBarHeight) / (Items.Count - ionView);
                                    Dif--;
                                }
                            }
                        }                
                    }
                }
                Offset = Math.Min(Offset, Items.Count - ionView);
                Offset = Math.Max(Offset, 0);
                oldMouseState = mouseState;
                if (ItemIndex > Items.Count)
                    ItemIndex = Items.Count - 1;
                if (ItemIndex < 0)
                    ItemIndex = 0;
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                if (yPerLine != -1)
                {
                    spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X,
                            (int)Pos.Y + (int)Parent.Pos.Y,
                            (int)(Size.X + xOffsetTextSize),
                            (int)Size.Y),
                        null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    if (Items.Count == 0)
                        spriteBatch.DrawString(
                            GUI.Fonts[Font],
                            "Lista vazia",
                            Pos + Parent.Pos + new Vector2(5, 0),
                            Color.DarkSlateGray * 0.64f);
                    for (int i = Offset; i < Math.Min(Offset + ionView, Items.Count); i++)
                    {
                        Vector2 tPos = Pos + Parent.Pos + new Vector2(2, (i - Offset) * yPerLine);
                        if (i == ItemIndex)
                            spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)tPos.X,
                                (int)tPos.Y+1,
                                (int)(Size.X - VerticalBarWidth - 5 + xOffsetTextSize),
                                (int)yPerLine),
                            null, Color.LightBlue, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        Color cor = TextColor;
                        if (!Enabled)
                            cor = Color.Gray;

                        int j = 0;
                        while ((j < Items[i].Length) &&
                            (GUI.Fonts[Font].MeasureString(Items[i].Substring(0, j)).X < Size.X + xOffsetTextSize - VerticalBarWidth))
                            j++;
                        if (j < Items[i].Length)
                        {
                            j -=4;
                            spriteBatch.DrawString(
                                GUI.Fonts[Font],
                                Items[i].Substring(0, j) + "...",
                                tPos,
                                cor);
                        }
                        else
                            spriteBatch.DrawString(
                                GUI.Fonts[Font],
                                Items[i].Substring(0, j),
                                tPos,
                                cor);
                    }
                    if (MouseOverBar)
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)(Pos.X + Parent.Pos.X + Size.X + xOffsetTextSize - VerticalBarWidth - 1),
                                (int)((Pos.Y + Parent.Pos.Y + 1) + (Offset / ((Items.Count - ionView) / (Size.Y - 2 - VerticalBarHeight)))),
                                (int)VerticalBarWidth,
                                (int)VerticalBarHeight),
                            null, verticalBarColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    else
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)(Pos.X + Parent.Pos.X + Size.X + xOffsetTextSize - VerticalBarWidth - 1),
                                (int)(( Pos.Y + Parent.Pos.Y + 1) + (Offset / ((Items.Count - ionView) / (Size.Y - 2 - VerticalBarHeight)))),
                                (int)VerticalBarWidth,
                                (int)VerticalBarHeight),
                            null, verticalBarColor * 0.72f, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                }
            }
            public void Add(string Text)
            {
                Items.Add(Text);
            }
        }
        // Uma checkbox
        public class TGUICheckBox : GUI.TGUIObject
        {
            public bool Checked, Changed;
            public bool MouseOver, Clicking;
            public int CheckBoxSize;
            public int CheckBoxYOffset;
            public Color backgroundColor;
            public bool transparentBackground;
            private MouseState oldMouseState;
            public TGUICheckBox(Vector2 Pos, Vector2 Size, string Text)
                : base(Pos, Size)
            {
                this.Text = Text;
                TextColor = Color.Black;
                AutoSize = true;
                transparentBackground = true;
                backgroundColor = Color.White;
                BorderSize = 0;
                CheckBoxSize = 20;
                CheckBoxYOffset = 2;
                MouseOver = false;
                Checked = false;
                Clicking = false;
                Visible = true;
                Enabled = true;
                Changed = false;
            }
            // Verifica clique do mouse no componente
            public override void Update(MouseState mouseState)
            {
                base.Update(mouseState);

                if (!Rectangle.Intersect(
                    new Rectangle(
                        (int)(this.Pos.X + Parent.Pos.X),
                        (int)(this.Pos.Y + Parent.Pos.Y),
                        (int)(Size.X) + CheckBoxSize,
                        (int)(Size.Y)),
                     new Rectangle(
                         (int)mouseState.X,
                         (int)mouseState.Y,
                         1, 1
                         )).IsEmpty)
                {
                    MouseOver = true;
                    if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                        (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))
                    {
                        Clicking = true;

                    }
                    if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        if (Clicking)
                        {
                            Checked = !Checked;
                            Changed = true;
                        }
                    }
                }
                else
                    MouseOver = false;
                if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    Clicking = false;
                }
                oldMouseState = mouseState;
                Clicking = Clicking && Enabled;
                MouseOver = MouseOver && Enabled;
            }
            // Desenha
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                if (!transparentBackground)
                    spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X + CheckBoxSize,
                            (int)Pos.Y + (int)Parent.Pos.Y + 2,
                            (int)Size.X,
                            (int)Size.Y-3),
                        null, backgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                if (Enabled)
                    spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos + Parent.Pos + new Vector2(CheckBoxSize, 2), TextColor);
                else
                    spriteBatch.DrawString(GUI.Fonts[Font], Text, Pos + Parent.Pos + new Vector2(CheckBoxSize, 2), Color.Gray);
                spriteBatch.Draw(wBox,
                    new Rectangle(
                        (int)Pos.X + (int)Parent.Pos.X,
                        (int)Pos.Y + (int)Parent.Pos.Y + CheckBoxYOffset,
                        (int)CheckBoxSize,
                        (int)CheckBoxSize),
                    null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                if (Enabled)
                    spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X + 1,
                            (int)Pos.Y + (int)Parent.Pos.Y + CheckBoxYOffset + 1,
                            (int)CheckBoxSize - 2,
                            (int)CheckBoxSize - 2),
                        null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X + 1,
                            (int)Pos.Y + (int)Parent.Pos.Y + CheckBoxYOffset + 1,
                            (int)CheckBoxSize - 2,
                            (int)CheckBoxSize - 2),
                        null, Color.SlateGray, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                if (Checked)
                    if (Enabled)
                        spriteBatch.Draw(CheckTex,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X + 1,
                            (int)Pos.Y + (int)Parent.Pos.Y + CheckBoxYOffset + 1,
                            (int)CheckBoxSize - 2,
                            (int)CheckBoxSize - 2),
                        null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    else
                        spriteBatch.Draw(CheckTex,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X + 1,
                            (int)Pos.Y + (int)Parent.Pos.Y + CheckBoxYOffset + 1,
                            (int)CheckBoxSize - 2,
                            (int)CheckBoxSize - 2),
                        null, Color.Gray, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }
        // Um Editbox
        public class TGUIEditBox : GUI.TGUIObject
        {
            public Color backgroundColor;
            public bool transparentBackground;
            private int cursorTimer;
            public bool hasFocus, mouseOver, ReadOnly, ResizeOnMouse;
            private bool drawCursor;
            private MouseState oldMouseState;
            private KeyboardState keyboardState, oldkeyboardState;
            private int cursorPos;
            private bool SelectAll;
            private int samekeyTimer;
            private Microsoft.Xna.Framework.Input.Keys sameKey;
            private bool capsLocked;
            private int xDrawSize, textDLength;
            public TGUIEditBox(Vector2 Pos, Vector2 Size, string DefaultText)
                : base(Pos, Size)
            {
                this.Text = DefaultText;
                TextColor = Color.Black;
                AutoSize = false;
                transparentBackground = false;
                backgroundColor = Color.Azure;
                BorderSize = 1;
                SelectAll = false;
                cursorPos = 0;
                hasFocus = false;
                sameKey = Microsoft.Xna.Framework.Input.Keys.Sleep;
                samekeyTimer = 0;
                drawCursor = false;
                cursorTimer = 0;
                Visible = true;
                Enabled = true;
                ReadOnly = false;
                ResizeOnMouse = true;
            }
            public override void Update(MouseState mouseState)
            {
                base.Update(mouseState);

                if (Enabled)
                {
                    if (!Rectangle.Intersect(
                            new Rectangle(
                                (int)(this.Pos.X + Parent.Pos.X),
                                (int)(this.Pos.Y + Parent.Pos.Y),
                                (int)(xDrawSize),
                                (int)(Size.Y)),
                                new Rectangle(
                                    (int)mouseState.X,
                                    (int)mouseState.Y,
                                    1, 1
                                    )).IsEmpty)
                        mouseOver = true;
                    else
                        mouseOver = false;

                    if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                            (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))
                    {
                        if (mouseOver) /*(!Rectangle.Intersect(
                            new Rectangle(
                                (int)(this.Pos.X + Parent.Pos.X),
                                (int)(this.Pos.Y + Parent.Pos.Y),
                                (int)(xDrawSize),
                                (int)(Size.Y)),
                             new Rectangle(
                                 (int)mouseState.X,
                                 (int)mouseState.Y,
                                 1, 1
                                 )).IsEmpty)*/
                        {
                            if (!hasFocus)
                            {
                                SelectAll = true;
                                cursorPos = Text.Length;
                                hasFocus = true;
                            }
                            else
                            {
                                SelectAll = false;
                                cursorTimer = 0;
                                int i = 0;
                                cursorPos = 0;
                                while (i <= textDLength)
                                {
                                    if ((mouseState.X - this.Pos.X + Parent.Pos.X - 2) < GUI.Fonts[Font].MeasureString(Text.Substring(0, i)).X)
                                        i = Text.Length + 10;
                                    i++;
                                    cursorPos++;
                                }
                                cursorPos--;
                            }
                        }
                        else
                        {
                            SelectAll = false;
                            cursorPos = 0;
                            hasFocus = false;
                            sameKey = Microsoft.Xna.Framework.Input.Keys.Sleep;
                            samekeyTimer = 0;
                            drawCursor = false;
                            cursorTimer = 0;
                        }
                    }
                    oldMouseState = mouseState;


                    keyboardState = Keyboard.GetState();
                    if (!ReadOnly)
                    {
                        if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.CapsLock))
                            if (oldkeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.CapsLock))
                                capsLocked = !capsLocked;
                        if (hasFocus)
                        {
                            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                            {
                                hasFocus = false;
                                SelectAll = false;
                                drawCursor = false;
                            }
                            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                            {
                                if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) ||
                                    keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                                {
                                }
                                else
                                {
                                    if (oldkeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Left))
                                    {
                                        samekeyTimer = 0;
                                        cursorPos--;
                                        cursorTimer = 0;
                                        drawCursor = true;
                                        SelectAll = false;
                                    }
                                    else
                                        samekeyTimer++;

                                    if (samekeyTimer > 30)
                                    {
                                        cursorPos--;
                                        samekeyTimer = 25;
                                        cursorTimer = 0;
                                        drawCursor = true;
                                        SelectAll = false;
                                    }
                                }
                            }
                            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                            {
                                if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) ||
                                    keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                                {
                                }
                                else
                                {
                                    if (oldkeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Right))
                                    {
                                        samekeyTimer = 0;
                                        cursorPos++;
                                        cursorTimer = 0;
                                        drawCursor = true;
                                        SelectAll = false;
                                    }
                                    else
                                        samekeyTimer++;

                                    if (samekeyTimer > 30)
                                    {
                                        cursorPos++;
                                        samekeyTimer = 20;
                                        cursorTimer = 0;
                                        drawCursor = true;
                                        SelectAll = false;
                                    }
                                }
                            }

                            foreach (Microsoft.Xna.Framework.Input.Keys key in keyboardState.GetPressedKeys())
                            {
                                if (oldkeyboardState.IsKeyDown(key))
                                    samekeyTimer++;
                                if ((oldkeyboardState.IsKeyUp(key) || (samekeyTimer > 20)) && (
                                    (key != Microsoft.Xna.Framework.Input.Keys.Left) && (key != Microsoft.Xna.Framework.Input.Keys.Right) &&
                                    (key != Microsoft.Xna.Framework.Input.Keys.LeftShift) && (key != Microsoft.Xna.Framework.Input.Keys.RightShift)
                                    ))
                                {
                                    if (key == Microsoft.Xna.Framework.Input.Keys.Back)
                                    {
                                        if (SelectAll)
                                        {
                                            Text = "";
                                            cursorPos = 0;
                                            SelectAll = false;
                                        }
                                        else
                                            if (cursorPos > 0)
                                            {
                                                Text = Text.Remove(cursorPos - 1, 1);
                                                cursorPos--;
                                            }
                                    }
                                    else
                                    {
                                        string adds = " ";
                                        if (key == Microsoft.Xna.Framework.Input.Keys.Space)
                                        {
                                            if (SelectAll)
                                            {
                                                Text = adds;
                                                cursorPos = 0;
                                                SelectAll = false;
                                            }
                                            Text = Text.Insert(cursorPos, adds);
                                        }
                                        else
                                        {
                                            adds = CharFromKey(key, keyboardState, capsLocked);
                                            Text = Text.Insert(cursorPos, adds);
                                            if ((SelectAll) && (adds.Length > 0))
                                            {
                                                Text = adds;
                                                cursorPos = 0;
                                                SelectAll = false;
                                            }
                                        }
                                        cursorPos += adds.Length;
                                    }
                                    if (key == sameKey)
                                        samekeyTimer = 12;
                                    else
                                    {
                                        samekeyTimer = 0;
                                        sameKey = key;
                                    }
                                }
                            }

                            cursorTimer++;
                            if (cursorTimer > 25)
                            {
                                drawCursor = !drawCursor;
                                cursorTimer = 0;
                            }
                        }
                    }
                }

                oldkeyboardState = keyboardState;

                cursorPos = Math.Min(cursorPos, Text.Length);
                cursorPos = Math.Max(cursorPos, 0);

                if (((hasFocus) || (mouseOver)) && (ResizeOnMouse))
                {
                    xDrawSize = Math.Max((int)GUI.Fonts[Font].MeasureString(Text).X + 3, (int)Size.X);
                    textDLength = Text.Length;
                }
                else
                {
                    xDrawSize = (int)Size.X;
                    int i = 0;
                    while ((i < Text.Length) &&
                        (GUI.Fonts[Font].MeasureString(Text.Substring(0, i)).X < xDrawSize))
                        i++;
                    if (i != Text.Length)
                        i--;
                    textDLength = i;
                }
            }
            private string CharFromKey(Microsoft.Xna.Framework.Input.Keys key, KeyboardState keyboardState, bool capsLocked)
            {
                switch (key)
                {
                    case (Microsoft.Xna.Framework.Input.Keys.OemCloseBrackets):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "'";
                        else
                            return "'";
                    case (Microsoft.Xna.Framework.Input.Keys.OemTilde):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "'";
                        else
                            return "'";
                    case (Microsoft.Xna.Framework.Input.Keys.A):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "A";
                        else
                            return "a";
                    case (Microsoft.Xna.Framework.Input.Keys.B):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "B";
                        else
                            return "b";
                    case (Microsoft.Xna.Framework.Input.Keys.C):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "C";
                        else
                            return "c";
                    case (Microsoft.Xna.Framework.Input.Keys.D):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "D";
                        else
                            return "d";
                    case (Microsoft.Xna.Framework.Input.Keys.E):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "E";
                        else
                            return "e";
                    case (Microsoft.Xna.Framework.Input.Keys.F):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "F";
                        else
                            return "f";
                    case (Microsoft.Xna.Framework.Input.Keys.G):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "G";
                        else
                            return "g";
                    case (Microsoft.Xna.Framework.Input.Keys.H):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "H";
                        else
                            return "h";
                    case (Microsoft.Xna.Framework.Input.Keys.I):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "I";
                        else
                            return "i";
                    case (Microsoft.Xna.Framework.Input.Keys.J):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "J";
                        else
                            return "j";
                    case (Microsoft.Xna.Framework.Input.Keys.K):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "K";
                        else
                            return "k";
                    case (Microsoft.Xna.Framework.Input.Keys.L):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "L";
                        else
                            return "l";
                    case (Microsoft.Xna.Framework.Input.Keys.M):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "M";
                        else
                            return "m";
                    case (Microsoft.Xna.Framework.Input.Keys.N):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "N";
                        else
                            return "n";
                    case (Microsoft.Xna.Framework.Input.Keys.O):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "O";
                        else
                            return "o";
                    case (Microsoft.Xna.Framework.Input.Keys.P):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "P";
                        else
                            return "p";
                    case (Microsoft.Xna.Framework.Input.Keys.Q):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "Q";
                        else
                            return "q";
                    case (Microsoft.Xna.Framework.Input.Keys.R):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "R";
                        else
                            return "r";
                    case (Microsoft.Xna.Framework.Input.Keys.S):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "S";
                        else
                            return "s";
                    case (Microsoft.Xna.Framework.Input.Keys.T):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "T";
                        else
                            return "t";
                    case (Microsoft.Xna.Framework.Input.Keys.U):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "U";
                        else
                            return "u";
                    case (Microsoft.Xna.Framework.Input.Keys.V):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "V";
                        else
                            return "v";
                    case (Microsoft.Xna.Framework.Input.Keys.W):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "W";
                        else
                            return "w";
                    case (Microsoft.Xna.Framework.Input.Keys.X):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "X";
                        else
                            return "x";
                    case (Microsoft.Xna.Framework.Input.Keys.Y):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "Y";
                        else
                            return "y";
                    case (Microsoft.Xna.Framework.Input.Keys.Z):
                        if ((keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            || (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) || capsLocked)
                            return "Z";
                        else
                            return "z";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad0):
                            return "0";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad1):
                            return "1";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad2):
                            return "2";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad3):
                            return "3";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad4):
                            return "4";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad5):
                            return "5";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad6):
                            return "6";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad7):
                            return "7";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad8):
                            return "8";
                    case (Microsoft.Xna.Framework.Input.Keys.NumPad9):
                            return "9";
                    case (Microsoft.Xna.Framework.Input.Keys.D0):
                            return "0";
                    case (Microsoft.Xna.Framework.Input.Keys.D1):
                            return "1";
                    case (Microsoft.Xna.Framework.Input.Keys.D2):
                            return "2";
                    case (Microsoft.Xna.Framework.Input.Keys.D3):
                            return "3";
                    case (Microsoft.Xna.Framework.Input.Keys.D4):
                            return "4";
                    case (Microsoft.Xna.Framework.Input.Keys.D5):
                            return "5";
                    case (Microsoft.Xna.Framework.Input.Keys.D6):
                            return "6";
                    case (Microsoft.Xna.Framework.Input.Keys.D7):
                            return "7";
                    case (Microsoft.Xna.Framework.Input.Keys.D8):
                            return "8";
                    case (Microsoft.Xna.Framework.Input.Keys.D9):
                            return "9";
                    default:
                        return "";
                }
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

               // if (ResizeSide == 1)
                {
                    if (!transparentBackground)
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)Pos.X + (int)Parent.Pos.X,
                                (int)Pos.Y + (int)Parent.Pos.Y,
                                (int)xDrawSize,
                                (int)Size.Y),
                            null, backgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    if (SelectAll)
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)Pos.X + (int)Parent.Pos.X + 2,
                                (int)Pos.Y + (int)Parent.Pos.Y + 2,
                                (int)GUI.Fonts[Font].MeasureString(Text.Substring(0, cursorPos)).X - 2,
                                (int)Size.Y - 5),
                            null, Color.LightBlue, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    Color cor = TextColor;
                    if (!Enabled)
                        cor = Color.Gray;
                    if ((textDLength < Text.Length) && (xDrawSize <= Size.X))
                        spriteBatch.DrawString(GUI.Fonts[Font], Text.Substring(0, textDLength - 2) + "...", Pos + Parent.Pos, cor);
                    else
                        spriteBatch.DrawString(GUI.Fonts[Font], Text.Substring(0, textDLength), Pos + Parent.Pos, cor);
                    if ((drawCursor) && (!SelectAll))
                        spriteBatch.DrawString(GUI.Fonts[Font], "|", Pos + Parent.Pos + new Vector2(GUI.Fonts[Font].MeasureString(Text.Substring(0, cursorPos)).X - 2, -2), cor);
                }
            }
        }
        // Um Memo
        public class TGUIMemo : GUI.TGUIObject
        {
            public List<string> Items;
            public int Offset;
            private int ionView;
            private int yPerLine;
            public Func<int, bool> OnClick;
            public Color color, verticalBarColor;
            public int VerticalBarWidth;
            private float VerticalBarHeight;
            public bool MouseOverList, MouseOverBar, Clicking;
            private MouseState oldMouseState;
            private Vector2 FirstClickPos;
            private float prevWheelValue, currWheelValue;
            public float xOffsetTextSize;
            public int listIndex;
            public TGUIMemo(Vector2 Pos, Vector2 Size)
                : base(Pos, Size)
            {
                Font = 2;
                Items = new List<string>();
                Offset = 0;
                yPerLine = -1;
                color = Color.White;
                TextColor = Color.Black;
                verticalBarColor = Color.SlateGray;
                VerticalBarWidth = 16;
                currWheelValue = 0f;
                prevWheelValue = 0f;
                Visible = true;
                Enabled = true;
                xOffsetTextSize = 0f;
                listIndex = 0;
            }
            public override void Update(MouseState mouseState)
            {
                base.Update(mouseState);
                if (yPerLine == -1)
                {
                    yPerLine = (int)(Fonts[Font].MeasureString("aAjgy1234567890").Y);
                    ionView = (int)(Size.Y / yPerLine);
                }
                VerticalBarHeight = (int)(Math.Pow((Size.Y), 2) / (ionView * Items.Count));
                if (VerticalBarHeight > Size.Y)
                    VerticalBarHeight = Size.Y - 2;

                prevWheelValue = currWheelValue;
                currWheelValue = mouseState.ScrollWheelValue;

                if (Enabled)
                {
                    if (!Rectangle.Intersect(
                        new Rectangle(
                            (int)(this.Pos.X + Parent.Pos.X),
                            (int)(this.Pos.Y + Parent.Pos.Y - 5),
                            (int)(Size.X + xOffsetTextSize + 10),
                            (int)(Size.Y)),
                         new Rectangle(
                             (int)mouseState.X,
                             (int)mouseState.Y,
                             1, 1
                             )).IsEmpty)
                    {
                        xOffsetTextSize = 0;
                        foreach (string s in Items)
                        {
                            float ns = Fonts[Font].MeasureString(s).X;
                            if (ns > xOffsetTextSize)
                                xOffsetTextSize = ns;
                        }
                        xOffsetTextSize = Math.Min(xOffsetTextSize, Size.X * 2);//AppGraphics.ScreenCenter.X);
                        if (xOffsetTextSize < Size.X)
                            xOffsetTextSize = 0;
                        else
                            xOffsetTextSize -= (Size.X - VerticalBarWidth - 5);

                        if (Rectangle.Intersect(
                        new Rectangle(
                                    (int)(Pos.X + Parent.Pos.X + xOffsetTextSize + Size.X - VerticalBarWidth - 1),
                                    (int)((Pos.Y + Parent.Pos.Y + 1)) - 5,
                                    (int)VerticalBarWidth,
                                    (int)Size.Y),
                         new Rectangle(
                             (int)mouseState.X,
                             (int)mouseState.Y,
                             1, 1
                             )).IsEmpty)
                        {
                            MouseOverList = true;
                            MouseOverBar = false;
                        }
                        else
                        {
                            if ((Rectangle.Intersect(
                                new Rectangle(
                                            (int)(Pos.X + Parent.Pos.X + xOffsetTextSize + Size.X - VerticalBarWidth - 1),
                                            (int)((Pos.Y + Parent.Pos.Y)) - 55,
                                            (int)VerticalBarWidth,
                                            (int)VerticalBarHeight + 10),
                                 new Rectangle(
                                     (int)mouseState.X,
                                     (int)mouseState.Y,
                                     1, 1
                                     )).IsEmpty)
                                && (((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                                (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))))
                            {
                                Offset = (int)
                                    -((Pos.Y + Parent.Pos.Y - mouseState.Y) / ((Size.Y - 2) / Items.Count));
                            }
                            MouseOverBar = true;
                            MouseOverList = false;
                        }
                        if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) &&
                            (oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) &&
                            (MouseOverBar))
                        {
                            Clicking = true;
                            FirstClickPos = new Vector2(mouseState.X, mouseState.Y);
                        }
                        Offset += (int)((prevWheelValue - currWheelValue) * 0.05);
                    }
                    else
                    {
                        if (Clicking == false)
                        {
                            xOffsetTextSize = 0;
                            MouseOverBar = false;
                            MouseOverList = false;
                        }
                    }

                    if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        Clicking = false;
                    }

                    if ((Clicking) && (MouseOverBar || MouseOverList))
                    {
                        double Dif = Math.Abs((FirstClickPos.Y - mouseState.Y)) / ((Size.Y - VerticalBarHeight) / (Items.Count - ionView));
                        if (Dif > 1)
                        {
                            if ((FirstClickPos.Y - mouseState.Y) > 0)
                            {
                                while (Math.Floor(Dif) > 1)
                                {
                                    Offset--;
                                    FirstClickPos.Y -= (Size.Y - VerticalBarHeight) / (Items.Count - ionView);
                                    Dif--;
                                }
                            }
                            else
                            {
                                while (Math.Floor(Dif) > 1)
                                {
                                    Offset++;
                                    FirstClickPos.Y += (Size.Y - VerticalBarHeight) / (Items.Count - ionView);
                                    Dif--;
                                }
                            }
                        }
                    }
                }

                Offset = Math.Min(Offset, Items.Count - ionView);
                Offset = Math.Max(Offset, 0);
                oldMouseState = mouseState;
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                if (yPerLine != -1)
                {
                    spriteBatch.Draw(wBox,
                        new Rectangle(
                            (int)Pos.X + (int)Parent.Pos.X,
                            (int)Pos.Y + (int)Parent.Pos.Y,
                            (int)(Size.X + xOffsetTextSize),
                            (int)Size.Y),
                        null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    for (int i = Offset; i < Math.Min(Offset + ionView, Items.Count); i++)
                    {
                        Vector2 tPos = Pos + Parent.Pos + new Vector2(2, (i - Offset) * yPerLine);
                        Color cor = TextColor;
                        if (!Enabled)
                            cor = Color.Gray;

                        int j = 0;
                        while ((j < Items[i].Length) &&
                            (GUI.Fonts[Font].MeasureString(Items[i].Substring(0, j)).X < Size.X + xOffsetTextSize - VerticalBarWidth))
                            j++;
                        if (j < Items[i].Length)
                        {
                            j -= 4;
                            spriteBatch.DrawString(
                                GUI.Fonts[Font],
                                Items[i].Substring(0, j) + "...",
                                tPos,
                                cor);
                        }
                        else
                            spriteBatch.DrawString(
                                GUI.Fonts[Font],
                                Items[i].Substring(0, j),
                                tPos,
                                cor);
                    }
                    if (MouseOverBar)
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)(Pos.X + Parent.Pos.X + Size.X + xOffsetTextSize - VerticalBarWidth - 1),
                                (int)((Pos.Y + Parent.Pos.Y + 1) + (Offset / ((Items.Count - ionView) / (Size.Y - 2 - VerticalBarHeight)))),
                                (int)VerticalBarWidth,
                                (int)VerticalBarHeight),
                            null, verticalBarColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    else
                        spriteBatch.Draw(wBox,
                            new Rectangle(
                                (int)(Pos.X + Parent.Pos.X + Size.X + xOffsetTextSize - VerticalBarWidth - 1),
                                (int)((Pos.Y + Parent.Pos.Y + 1) + (Offset / ((Items.Count - ionView) / (Size.Y - 2 - VerticalBarHeight)))),
                                (int)VerticalBarWidth,
                                (int)VerticalBarHeight),
                            null, verticalBarColor * 0.72f, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                }
            }
            public void Add(string Text, bool Ident)
            {
                Items.Add("");
                foreach (string Subs in Text.Split(' '))
                {
                    if (Fonts[Font].MeasureString(Items[listIndex] + Subs + " ").X < Size.X - VerticalBarWidth)
                    {
                        Items[listIndex] += Subs + " ";
                    }
                    else
                    {
                        if (Ident)
                            Items.Add(" " + Subs + " ");
                        else
                            Items.Add(Subs + " ");
                        listIndex++;
                    }
                }
                listIndex++;
            }
        public void Clear()
        {
            Items.Clear();
            listIndex = 0;
        }
        }
        // Uma imagem (textura 2d)
        public class TGUIImage : GUI.TGUIObject
        {
            public Texture2D Image;
            public Color color;
            public TGUIImage(Vector2 Pos, Vector2 Size, Texture2D Image)
                : base(Pos, Size)
            {
                this.Image = Image;
                color = Color.White;
                Visible = true;
                Enabled = true;
            }
            // Desenha
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                spriteBatch.Draw(Image,
                    new Rectangle(
                        (int)Pos.X + (int)Parent.Pos.X,
                        (int)Pos.Y + (int)Parent.Pos.Y,
                        (int)Size.X,
                        (int)Size.Y),
                    null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }
    }
}
