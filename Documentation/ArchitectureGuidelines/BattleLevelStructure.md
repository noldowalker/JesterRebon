# Архитектура боевого уровня

У боевой сцены есть корневой объект, который должен носить название BootAndRoot.
Все содержимое сцены помещается как дочерние объекты:

![img.png](./Img/level_boot_and_root_in_hierarchy.png)

Сам объект в качестве компонентов содержит Глобальные системы и Контейнер для инверсии зависимостей.

![img.png](./Img/level_boor_and_root_example.png)

##Контейнер инверсии зависимостей
Реализация наследника LifetimeScope для конкретной сцены в рамках [vContainer](https://vcontainer.hadashikick.jp/)
Для каждого типа проброса зависимостей выделены свои регионы, пожалуйста соблюдаем их. Если появилась новая категоряи объектов, выделяем для нее отдельную зону.


Собирает и пробрасывает все зависимости в иерархии, для которых у него указано

    builder.RegisterComponentInHierarchy<Объект_участвующий_в_DI>();

Все необходимые для работы уровня ScriptableObjects должны передаваться через внешние поля данного объекта.
Таким образом сразу становистя понятно, какие ресурсы использует данный уровень, а так же их замену для будет осуществить
весьма просто и в одном месте. 

    public class SceneContext : LifetimeScope
    {
            [SerializeField] private SomeScriptableObject someScriptableObject;
    
            protected override void Configure(IContainerBuilder builder)
            {
                //...
                
                #region Scriptable Objects for Injection
        
                builder.RegisterInstance(someScriptableObject);
        
                #endregion Scriptable Objects for Injection
                
                //...
            }  
    }

Для классов создаваемых на сцене через конструктор и пробрасываемых через DI срок жизни (Lifetime) устанавливаем как Scope и тогда экземпляр будет уничтожаться с уничтожением конневого объекта сцены.

[На главную](../../README.md)
