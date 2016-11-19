using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAF
{
    public enum InfluencerType
    {
        Kimble,
        Whitfield,
        Nandos,
        ProjectLaunch,
        SalaryIncrease,
        FixedPriceSales
    }

    public class FAFInfluencer : ICloneable
    {
        const int BaseSpeed = 2;
        
        static FAFSprite StateSpriteGood, StateSpriteBad;

        public static void Init(GraphicsDevice gd)
        {
            StateSpriteGood = new FAFSprite("Content/InfluencerStateGood.png");
            StateSpriteBad = new FAFSprite("Content/InfluencerStateBad.png");
            StateSpriteGood.LoadContent(gd);
            StateSpriteBad.LoadContent(gd);
        }

        public FAFSprite Sprite
        {
            get;
            private set;
        }

        public int PointsOnCollision
        {
            get;
            set;
        }


        int speed;
        public int Speed
        {
            get
            {
                return BaseSpeed + speed;
            }
            set
            {
                speed = value;
            }
        }

        public int VariableYAmount
        {
            get;
            set;
        }

        int currentVariableYAmount;
        bool isVariableIncreasing;

        TimeSpan KillTime
        {
            get;
            set;
        }

        public bool PointsAwarded
        {
            get;
            set;
        }

        public bool IsKilled(GameTime gt)
        {
            return KillTime != TimeSpan.Zero && gt.TotalGameTime >= KillTime;
        }

        public void SetKilled(GameTime gt)
        {
            var sp = (FAFSprite)(PointsOnCollision > 0 ? StateSpriteGood.Clone() : StateSpriteBad.Clone());
            sp.Position = Sprite.Position;
            Sprite = sp;
            KillTime = gt.TotalGameTime.Add(TimeSpan.FromSeconds(1));
        }

        public int GetYDelta()
        {
            if (VariableYAmount > 0)
            {
                if (isVariableIncreasing)
                {
                    currentVariableYAmount--;
                    if (currentVariableYAmount <= -VariableYAmount)
                    {
                        // change direction
                        isVariableIncreasing = false;
                    }
                }
                else
                {
                    currentVariableYAmount++;
                    if (currentVariableYAmount >= VariableYAmount)
                    {
                        isVariableIncreasing = true;
                    }
                }
                return currentVariableYAmount;
            }
            return 0;
        }

        public FAFInfluencer(FAFSprite sp, GraphicsDevice gd)
        {
            Sprite = sp;
            sp.LoadContent(gd);
        }

        FAFInfluencer(FAFSprite sp)
        {
            Sprite = sp;
        }

        public object Clone()
        {
            return new FAFInfluencer((FAFSprite)Sprite.Clone())
            {
                Speed = Speed,
                PointsOnCollision = PointsOnCollision,
                VariableYAmount = VariableYAmount
            };
        }
    }
}
