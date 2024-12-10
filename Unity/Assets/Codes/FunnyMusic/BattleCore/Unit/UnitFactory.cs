using Cysharp.Threading.Tasks;
using Framework;

namespace FunnyMusic
{
    public static class UnitFactory
    {
        public static async UniTask<Unit> Create(UnitInfo unitInfo)
        {
            UniTaskCompletionSource<Unit> tcs = new UniTaskCompletionSource<Unit>();
               
            
            UnitComponent unitComponent = GameWorld.World.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            GameObjectComponent gameObjectComponent = unit.AddComponent<GameObjectComponent>();
            unit.AddComponent<AnimatorComponent>();
            await gameObjectComponent.LoadGameObject();
            unitComponent.Add(unit);

            tcs.TrySetResult(unit);
            
            return await tcs.Task;
        }

        public static async UniTask<PlayerComponent> CreatePlayer(UnitInfo unitInfo)
        {
            UniTaskCompletionSource<PlayerComponent> tcs = new UniTaskCompletionSource<PlayerComponent>();

            Unit unit = await Create(unitInfo);

            PlayerComponent playerComponent = unit.AddComponent<PlayerComponent>();
         
            tcs.TrySetResult(playerComponent);
            
            return await tcs.Task;
        }
    }
    
}