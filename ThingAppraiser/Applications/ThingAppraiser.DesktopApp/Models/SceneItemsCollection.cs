using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models
{
    internal sealed class SceneItemsCollection
    {
        private readonly List<SceneItem> _sceneItems;

        public IReadOnlyList<SceneItem> SceneItems => _sceneItems;


        public SceneItemsCollection()
        {
            _sceneItems = new List<SceneItem>(capacity: 16);
        }

        public void AddScene(string sceneName, UserControl content)
        {
            sceneName.ThrowIfNullOrEmpty(nameof(sceneName));
            content.ThrowIfNull(nameof(content));

            if (Contains(sceneName))
            {
                throw new ArgumentException(
                    $"Scene with name \"{sceneName}\" has already been added to the scene " +
                    $"collection.",
                    nameof(sceneName)
                );
            }

            var newScene = new SceneItem(sceneName, content);
            _sceneItems.Add(newScene);
        }

        public bool Contains(string sceneName)
        {
            sceneName.ThrowIfNullOrEmpty(nameof(sceneName));

            return FindSceneIndex(sceneName) != Constants.NotFoundIndex;
           
        }

        public int FindSceneIndex(string sceneName)
        {
            sceneName.ThrowIfNullOrEmpty(nameof(sceneName));

            return _sceneItems
                .IndexOf(item =>
                    string.Equals(item.Name, sceneName, StringComparison.OrdinalIgnoreCase)
                );
        }

        public SceneItem GetSceneItem(string sceneName)
        {
            sceneName.ThrowIfNullOrEmpty(nameof(sceneName));

            int index = FindSceneIndex(sceneName);
            if (index == Constants.NotFoundIndex)
            {
                throw new ArgumentException(
                    $"Scene with name \"{sceneName}\" was not found in the scene collection.",
                    nameof(sceneName)
                );
            }

            return _sceneItems[index];
        }

        public T GetControl<T>(string sceneName)
            where T : UserControl
        {
            sceneName.ThrowIfNullOrEmpty(nameof(sceneName));

            SceneItem sceneItem = GetSceneItem(sceneName);
            if (!(sceneItem.Content is T control))
            {
                throw new ArgumentException(
                    $"Content of scene with name \"{sceneName}\" is not convertable to " +
                    $"{typeof(T).FullName}.",
                    nameof(sceneName)
                );
            }

            return control;
        }

        public T GetDataContext<T>(string sceneName)
        {
            sceneName.ThrowIfNullOrEmpty(nameof(sceneName));

            SceneItem sceneItem = GetSceneItem(sceneName);
            if (!(sceneItem.Content.DataContext is T control))
            {
                throw new ArgumentException(
                    $"Content of scene with name \"{sceneName}\" is not convertable to " +
                    $"{typeof(T).FullName}.",
                    nameof(sceneName)
                );
            }

            return control;
        }
    }
}
