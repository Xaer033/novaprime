using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GhostGen
{
    public class ViewFactory : IViewFactory<UIView>
    {
        public Canvas canvas { get; set; }
        
        public class AsyncViewResult : MultiMap<string, UIView>
        {
        }
        
        internal class AsyncBlock
        {
            public static AsyncBlock Create<T>(string name, ResourceRequest p_request, Action<T> p_callback, Transform p_parent) where T : UIView
            {
                AsyncBlock block = new AsyncBlock();
                block.name = name;
                block.request = p_request;
                block.parent = p_parent;
                block.callback = (view) =>
                {
                    if(p_callback != null)
                    {
                        p_callback((T)view);
                    }
                };
                return block;
            }

            public ResourceRequest request;
            public Action<UIView> callback;
            public Transform parent;
            public string name;

        }
        
        private List<AsyncBlock> _asyncList = new List<AsyncBlock>();
        
        public ViewFactory(Canvas guiCanvas)
        {
            canvas = guiCanvas;
        }   
   
        public void Step()
        {
            int asyncLength = _asyncList.Count;
            for(int i = 0; i < asyncLength; ++i)
            { 
                var block = _asyncList[i];
                Assert.IsNotNull(block);
                Assert.IsNotNull(block.request);
            
                if(!block.request.isDone) { continue; }

                Assert.IsNotNull(block.request.asset, "Asset: " + block.name + " couldn't be loaded!");

                UIView view = _createView((UIView)block.request.asset, block.parent);
                Assert.IsNotNull(view);
            
                if(block.callback != null)
                {
                    block.callback(view);
                }

                _asyncList[i] = null;        
            }

            for(int i = asyncLength-1; i >=0; --i)
            {
                if(_asyncList[i] == null)
                {
                    _asyncList.RemoveAt(i);
                }
            }
        }

        public T GetPrefab<T>(string viewPath) where T : UIView
        {
            return (T)Resources.Load<UIView>(viewPath);
        }

        public T Create<T>(AssetRequest request) where T : UIView
        {
            Assert.IsFalse(String.IsNullOrEmpty(request.path));

            UIView viewBase = Resources.Load<UIView>(request.path);
            Assert.IsNotNull(viewBase);
        
            return (T)_createView(viewBase, request.parent);
        }

        public T Create<T>(UIView prefab, Transform parent = null) where T : UIView
        {
            Assert.IsNotNull(prefab);
            return (T)_createView(prefab, parent);
        }

        public bool CreateAsync<T>(string viewPath, Action<T> callback) where T : UIView
        {
            Assert.IsFalse(String.IsNullOrEmpty(viewPath));
            AssetRequest assetRequest = new AssetRequest(viewPath);
            ResourceRequest request = Resources.LoadAsync<T>(assetRequest.path);

            if(request == null) { return false; }

            AsyncBlock block = AsyncBlock.Create<T>(assetRequest.path, request, callback, assetRequest.parent);
            _asyncList.Add(block);

            return true;
        }

        public bool CreateAsync<T>(AssetRequest assetRequest, Action<T> callback) where T : UIView
        {
            Assert.IsFalse(String.IsNullOrEmpty(assetRequest.path));

            ResourceRequest request = Resources.LoadAsync<T>(assetRequest.path);

            if (request == null) { return false; }

            AsyncBlock block = AsyncBlock.Create<T>(assetRequest.path, request, callback, assetRequest.parent);
            _asyncList.Add(block);

            return true;
        }

        public bool CreateAsyncFromList(List<AssetRequest> requestList, Action<AsyncViewResult> callback)
        {
            Assert.IsNotNull(requestList);
            bool result = true;

            AsyncViewResult resultMap = new AsyncViewResult();
            int viewsToLoadCount = requestList.Count;

            for(int i = 0; i < requestList.Count; ++i)
            {           
                AssetRequest request = requestList[i];
                Assert.IsFalse(String.IsNullOrEmpty(request.path));

                result = CreateAsync<UIView>(request, (view) =>
                {
                    viewsToLoadCount--;

                    resultMap.Add(request.path, view);

                    if(viewsToLoadCount <= 0 && callback != null)
                    {
                        callback(resultMap);
                    }
                });

                if(!result)
                {
                    Debug.LogError("Error trying to create view: " + request.path);
                    break;
                }
            }

            return result;
        }

        public void RemoveView(UIView view)
        {
            Assert.IsNotNull(view);
            _removeView(view);
        }

        private void _removeView(UIView view)
        {
            if(view != null)
            {
                view.RemoveAllListeners();
                view.OnViewDispose();
                GameObject.Destroy(view.gameObject);
                view = null;
            }
        }

        private UIView _createView(UIView viewBase, Transform parent)
        {
            Transform viewParent = (parent != null) ? parent : canvas.transform;
            Assert.IsNotNull(viewBase);
            return GameObject.Instantiate<UIView>(viewBase, viewParent, false);
        }
    }
}
