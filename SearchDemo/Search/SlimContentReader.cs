using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;

namespace SearchDemo.Search
{
    /// <summary>
    /// Pulled from EPiServer Core and modified to index all content (not just published)
    /// 
    /// </summary>
    internal class SlimContentReader
    {
        private Stack<ContentReference> _backlog = new Stack<ContentReference>();
        private Queue<IContent> _queue = new Queue<IContent>();
        private IContentRepository _contentRepository;
        private LanguageSelectorFactory _languageSelectorFactory;
        private Func<IContent, bool> _traverseChildren;

        public IContent Current { get; private set; }

        public SlimContentReader(IContentRepository contentRepository, LanguageSelectorFactory languageSelectorFactory, ContentReference start)
            : this(contentRepository, languageSelectorFactory, start, (Func<IContent, bool>)(c => true))
        {
        }

        public SlimContentReader(IContentRepository contentRepository, LanguageSelectorFactory languageSelectorFactory, ContentReference start, Func<IContent, bool> traverseChildren)
        {
            this._contentRepository = contentRepository;
            this._languageSelectorFactory = languageSelectorFactory;
            this._backlog.Push(start);
            this._traverseChildren = traverseChildren;
        }

        public bool Next()
        {
            if (this._backlog.Count == 0 && this._queue.Count == 0)
                return false;
            if (this._queue.Count == 0)
            {
                bool flag = true;
                ContentReference contentLink = this._backlog.Pop();
                foreach (IContent content in this._contentRepository.GetLanguageBranches<IContent>(contentLink))
                {
                    flag = flag & this._traverseChildren(content);
                    this._queue.Enqueue(content);
                }
                if (flag)
                {
                    IContent[] contentArray = Enumerable.ToArray<IContent>(this._contentRepository.GetChildren<IContent>(contentLink, this._languageSelectorFactory.MasterLanguage()));
                    for (int length = contentArray.Length; length > 0; --length)
                        this._backlog.Push(new ContentReference(contentArray[length - 1].ContentLink.ID, contentArray[length - 1].ContentLink.ProviderName));
                }
            }
            this.Current = this._queue.Dequeue();
            return true;
        }
    }
}
