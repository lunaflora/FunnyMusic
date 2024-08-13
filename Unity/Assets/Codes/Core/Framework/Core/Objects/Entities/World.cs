using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// 原则上为游戏世界的最上层Entity
    /// 其他所有Entity的祖先，需要一些特殊处理
    /// </summary>
    [EnableMethod]
    public sealed class World : Entity
    {

        public string Namne
        {
            get;
            set;
        }

        public World(long instanceId, string name, Entity parent)
        {
            this.Id = instanceId;
            this.InstanceId = instanceId;
            this.Namne = name;
            this.parent = parent;
            this.BeCreated = true;
            this.BeNew = true;
            this.BeRegister = true;
            this.Domain = this;
            CustomLogger.Log(LoggerLevel.Log,$"create world {this.Namne} {this.Id} ");
        }

        public override void Dispose()
        {
            base.Dispose();
            CustomLogger.Log(LoggerLevel.Log,$"dispose world {this.Namne} {this.Id} ");
        }

        public Scene GetScene(long id)
        {
            if (this.Children == null)
            {
                return null;
            }
            
            if (!this.Children.TryGetValue(id, out Entity scene))
            {
                return null;
            }
            
            return scene as Scene;
            
            
        }

        public new Entity Domain
        {
            get => this.domain;
            set => this.domain = value;
        }

        public new Entity Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (value == null)
                {
                    this.parent = this;
                    return;
                }

                this.parent = value;
                this.parent.Children.Add(this.Id,this);
            }
            
        }
    }
}