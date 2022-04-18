using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ListScroller : InteractiveElement
{
    [SerializeField] private RectTransform _parentRect = null;
    [SerializeField] private GameObject _scrollElementPrefab = null;
    [SerializeField] private ScrollerParameters _scrollerParameters = null;
    [SerializeField] private float _maxSize = 0.0f;
    [SerializeField] private float _fixedBottomPadding = 0.0f;
    [SerializeField] private RectTransform _sizeRect = null;
    [SerializeField] private float _sizeRectPadding = 10.0f;
    [SerializeField] private RectTransform _emptyListObject = null;
    [SerializeField] private Vector2 _direction = Vector2.down;
    [SerializeField, Range(1, 10)] private byte _rows = 1;
    [SerializeField] private bool _infinite = false;
    [SerializeField] private RectTransform _scrollerParent = null;
    [SerializeField] private RectTransform _scroller = null;

    private bool _directionHorizontal = false;
    private bool _directionRight = false;
    private bool _directionLeft = false;
    private bool _directionUp = false;
    private bool _directionDown = false;

    private bool _parametersInit = false;

    private float _rectHeight = 0.0f;
    private float _rectWidth = 0.0f;
    private float _parentTop = 0.0f;
    private float _parentBottom = 0.0f;
    private float _parentLeft = 0.0f;
    private float _parentRight = 0.0f;
    private float _parentHeight = 0.0f;
    private float _parentWidth = 0.0f;

    private int _elementCount = -1;
    private int _maxElementCount = 0;
    private int _indexOffset = 0;
    private int _startIndex = 0;

    private bool _scrollable = false;
    private float _scrollStartTime = 0.0f;
    private float _clickDifference = 0.0f;

    private float _lastClickPos = 0.0f;
    private Vector2 _startClickPos = Vector2.zero;
    private bool _startedDragging = false;

    protected List<ScrollableElement> scrollableElements = new List<ScrollableElement>();
    private List<object> _initialData = new List<object>();
    private List<object> _scrollData = new List<object>();

    void InitParameters()
    {
        SetDirections();
        SetParentDimensions();
        SetParentPivot(_parentRect);

        GameObject scrollableGameObject = Instantiate(_scrollElementPrefab, _parentRect);

        scrollableGameObject.SetActive(false);
        ScrollableElement scrollableElement = scrollableGameObject.GetComponent<ScrollableElement>();
        scrollableElements.Add(scrollableElement);
        SetElementPivotAnchor(scrollableElement.parentRect);
        scrollableElement.parentRect.anchoredPosition = Vector2.zero;

        _rectHeight = scrollableElement.parentRect.rect.height / CanvasScaleManager.screenScalerRatio.y;
        _rectWidth = scrollableElement.parentRect.rect.width / CanvasScaleManager.screenScalerRatio.x;

        SetMaxElementCount();
    }

    void SetParentPivot(RectTransform rect)
    {
        float bottom = UIHelpers.GetRectBottom(rect);
        float left = UIHelpers.GetRectLeft(rect);

        if (_directionRight)
            rect.pivot = new Vector2(0.0f, rect.pivot.y);
        else if (_directionLeft)
            rect.pivot = new Vector2(1.0f, rect.pivot.y);
        else if (_directionUp)
            rect.pivot = new Vector2(rect.pivot.x, 0.0f);
        else if (_directionDown)
            rect.pivot = new Vector2(rect.pivot.x, 1.0f);

        rect.position = new Vector2(
            left + rect.pivot.x * _parentWidth,
            bottom + rect.pivot.y * _parentHeight);
    }

    void SetElementPivotAnchor(RectTransform rect)
    {
        Vector2 corner = Vector2.zero;

        if (_directionRight)
            corner = new Vector2(0.0f, 1.0f);
        else if (_directionLeft)
            corner = new Vector2(1.0f, 1.0f);
        else if (_directionUp)
            corner = new Vector2(0.0f, 0.0f);
        else if (_directionDown)
            corner = new Vector2(0.0f, 1.0f);

        rect.pivot = rect.anchorMin = rect.anchorMax = corner;
    }

    void SetDirections()
    {
        _directionHorizontal = Mathf.Abs(_direction.x) > Mathf.Epsilon;
        _directionRight = false;
        _directionLeft = false;
        _directionUp = false;
        _directionDown = false;

        if (_direction.x > Mathf.Epsilon)
            _directionRight = true;
        else if (-_direction.x > Mathf.Epsilon)
            _directionLeft = true;
        else if (_direction.y > Mathf.Epsilon)
            _directionUp = true;
        else if (-_direction.y > Mathf.Epsilon)
            _directionDown = true;
    }

    void SetMaxElementCount()
    {
        if (_directionHorizontal)
            _maxElementCount = Mathf.FloorToInt(GetMaxSize() / _rectWidth) * _rows;
        else
            _maxElementCount = Mathf.FloorToInt(GetMaxSize() / _rectHeight) * _rows;
    }

    void SetParentDimensions()
    {
        _parentTop = UIHelpers.GetRectTop(_parentRect);
        _parentBottom = UIHelpers.GetRectBottom(_parentRect);
        _parentLeft = UIHelpers.GetRectLeft(_parentRect);
        _parentRight = UIHelpers.GetRectRight(_parentRect);
        _parentHeight = _parentTop - _parentBottom;
        _parentWidth = _parentRight - _parentLeft;
    }

    float GetMaxSize()
    {
        if (_maxSize > Mathf.Epsilon)
        {
            if (_directionHorizontal)
                return _maxSize / CanvasScaleManager.screenScalerRatio.x;
            else
                return _maxSize / CanvasScaleManager.screenScalerRatio.y;
        }

        if (_directionRight)
        {
            if (_sizeRect == null)
                return CanvasScaleManager.screenSize.x - _parentLeft;
            else
                return (UIHelpers.GetRectLeft(_sizeRect) - (_sizeRectPadding / CanvasScaleManager.screenScalerRatio.x)) - _parentLeft;
        }
        else if (_directionLeft)
        {
            if (_sizeRect == null)
                return _parentRight;
            else
                return _parentRight - (UIHelpers.GetRectRight(_sizeRect) + (_sizeRectPadding / CanvasScaleManager.screenScalerRatio.x));
        }
        else if (_directionUp)
        {
            if (_sizeRect == null)
                return CanvasScaleManager.screenSize.y - _parentBottom;
            else
                return (UIHelpers.GetRectBottom(_sizeRect) - (_sizeRectPadding / CanvasScaleManager.screenScalerRatio.y)) - _parentBottom;
        }
        else if (_directionDown)
        {
            if (_sizeRect == null)
            {
                if (_fixedBottomPadding > 0.0f)
                    return CanvasScaleManager.screenSize.y - (_fixedBottomPadding / CanvasScaleManager.screenScalerRatio.y) - (CanvasScaleManager.screenSize.y - _parentTop);

                return _parentTop;
            }
            else
                return _parentTop - (UIHelpers.GetRectTop(_sizeRect) + (_sizeRectPadding / CanvasScaleManager.screenScalerRatio.y));
        }

        return 0.0f;
    }

    public void InitScroll()
    {
        _scrollStartTime = Time.time + _scrollerParameters.scrollDelayPostInit;

        if (_parametersInit)
            return;

        _parametersInit = true;
        InitParameters();
    }

    public void ResetScrollPosition()
    {
        _indexOffset = GetStartIndex();
        _clickDifference = 0.0f;
        _startedDragging = false;

        for (int i = 0, count = scrollableElements.Count; i < count; ++i)
        {
            if (_directionRight)
            {
                scrollableElements[i].parentRect.position = new Vector2(
                    _parentLeft + ((i / _rows) * _rectWidth),
                    _parentTop - ((i % _rows) * _rectHeight));
            }
            else if (_directionLeft)
            {
                scrollableElements[i].parentRect.position = new Vector2(
                    _parentRight - ((i / _rows) * _rectWidth),
                    _parentTop - ((i % _rows) * _rectHeight));
            }
            else if (_directionUp)
            {
                scrollableElements[i].parentRect.position = new Vector2(
                    _parentLeft + ((i % _rows) * _rectWidth),
                    _parentBottom + ((i / _rows) * _rectHeight));
            }
            else if (_directionDown)
            {
                scrollableElements[i].parentRect.position = new Vector2(
                    _parentLeft + ((i % _rows) * _rectWidth),
                    _parentTop - ((i / _rows) * _rectHeight));
            }
        }

        ClampElementsToMax();
        UpdateElementContent();
    }

    public void UpdateElementContent()
    {
        int count = scrollableElements.Count;

        for (int i = 0; i < count; ++i)
        {
            if (_infinite)
            {
                int finalOffset = (i + _indexOffset) % _scrollData.Count;

                if (finalOffset < 0)
                    finalOffset = _scrollData.Count + finalOffset;

                scrollableElements[i].SetContent(_scrollData[finalOffset]);
            }
            else
            {
                if (i + _indexOffset >= _scrollData.Count)
                    scrollableElements[i].gameObject.SetActive(false);
                else
                {
                    scrollableElements[i].SetContent(_scrollData[i + _indexOffset]);
                    scrollableElements[i].gameObject.SetActive(true);
                }
            }
        }

        UpdateScrollerPosition();
    }

    void ClampIndexOffset()
    {
        if (_indexOffset < 0)
            _indexOffset = 0;
        else
        {
            int lastIndexOffset = _elementCount - _rows;

            if (_indexOffset > lastIndexOffset)
            {
                _indexOffset = lastIndexOffset;
                return;
            }
        }

        _indexOffset -= _indexOffset % _rows;
    }

    public int GetDataCount() => _scrollData.Count;

    public void SetData(List<object> data)
    {
        if (_initialData == data)
            return;

        _initialData = data;
        UpdateData();
    }

    protected virtual int GetStartIndex() => _startIndex;

    public void SetStartIndex(int index) => _startIndex = index;

    public void ScrollToIndex(int index)
    {
        int previousIndex = _startIndex;
        _startIndex = index;
        ResetScrollPosition();
        _startIndex = previousIndex;
    }

    protected virtual List<object> FilterData(List<object> data) => data;

    protected virtual List<object> SortData(List<object> data) => data;

    public void ClearData()
    {
        _initialData.Clear();
        _scrollData.Clear();
        UpdateData();
    }

    public void UpdateData()
    {
        InitScroll();
        _scrollData = FilterData(new List<object>(_initialData));

        int dataCount = _scrollData.Count;

        if (dataCount > 0)
        {
            if (dataCount % _rows != 0)
                dataCount += _rows - (dataCount % _rows);

            int targetCount = GetTargetElementCount(_elementCount);
            int newTargetCount = GetTargetElementCount(dataCount);

            _scrollData = SortData(_scrollData);

            if (dataCount == _elementCount || targetCount == newTargetCount)
            {
                _elementCount = dataCount;

                ResetScrollPosition();

                for (int i = 0, count = scrollableElements.Count; i < count; ++i)
                    scrollableElements[i].SetCreated();

                return;
            }

            _elementCount = dataCount;

            if (_emptyListObject)
                _emptyListObject.gameObject.SetActive(false);

            if (_elementCount > _maxElementCount)
            {
                _scrollable = true;

                float size = GetMaxSize() * CanvasScaleManager.screenScalerRatio.x;

                if (_directionHorizontal)
                    _parentRect.sizeDelta = new Vector2(size / _parentRect.localScale.x, _parentRect.sizeDelta.y);
                else
                    _parentRect.sizeDelta = new Vector2(_parentRect.sizeDelta.x, size / _parentRect.localScale.y);
            }
            else
            {
                _scrollable = false;
                _indexOffset = 0;

                if (_directionHorizontal)
                {
                    float targetSize = _rectWidth * _elementCount / _rows;
                    _parentRect.sizeDelta = new Vector2(targetSize / _parentRect.lossyScale.x, _parentRect.sizeDelta.y);
                }
                else
                {
                    float targetSize = _rectHeight * _elementCount / _rows;
                    _parentRect.sizeDelta = new Vector2(_parentRect.sizeDelta.x, targetSize / _parentRect.lossyScale.y);
                }
            }

            SetParentDimensions();
            EnsureElementCount();
            ResetScrollPosition();

            for (int i = 0, count = scrollableElements.Count; i < count; ++i)
                scrollableElements[i].SetCreated();
        }
        else
        {
            if (_elementCount == 0)
                return;

            _elementCount = 0;

            for (int i = 0, count = scrollableElements.Count; i < count; ++i)
                scrollableElements[i].gameObject.SetActive(false);

            _scrollable = false;

            if (_emptyListObject)
            {
                _emptyListObject.gameObject.SetActive(true);

                if (_directionHorizontal)
                    _parentRect.sizeDelta = new Vector2(_emptyListObject.sizeDelta.x, _parentRect.sizeDelta.y);
                else
                    _parentRect.sizeDelta = new Vector2(_parentRect.sizeDelta.x, _emptyListObject.sizeDelta.y);
            }
            else
            {
                if (_directionHorizontal)
                    _parentRect.sizeDelta = new Vector2(0.0f, _parentRect.sizeDelta.y);
                else
                    _parentRect.sizeDelta = new Vector2(_parentRect.sizeDelta.x, 0.0f);
            }

            SetParentDimensions();
        }
    }

    void ClampElementsToMax()
    {
        int count = 0;

        if (!_infinite)
        {
            ClampIndexOffset();

            int lastIndexOffset = Mathf.Max(_elementCount - scrollableElements.Count, 0);

            if (_indexOffset > lastIndexOffset)
            {
                count = Mathf.CeilToInt((_indexOffset - lastIndexOffset) / (float)_rows);
                _indexOffset = lastIndexOffset;

                float newOffset = _directionHorizontal ?
                        _rectWidth :
                        _rectHeight;

                newOffset *= count;

                if (_directionRight || _directionUp)
                    newOffset *= -1.0f;

                foreach (ScrollableElement element in scrollableElements)
                {
                    element.parentRect.position += (Vector3)(_directionHorizontal ?
                        Vector2.right * newOffset :
                        Vector2.up * newOffset);
                }

                float lastHeight = 0.0f;

                if (_directionRight)
                    lastHeight = _parentRight - (scrollableElements[scrollableElements.Count - 1].parentRect.position.x + _rectWidth);
                else if (_directionLeft)
                    lastHeight = (scrollableElements[scrollableElements.Count - 1].parentRect.position.x - _rectWidth) - _parentLeft;
                else if (_directionUp)
                    lastHeight = _parentTop - (scrollableElements[scrollableElements.Count - 1].parentRect.position.y + _rectHeight);
                else if (_directionDown)
                    lastHeight = (scrollableElements[scrollableElements.Count - 1].parentRect.position.y - _rectHeight) - _parentBottom;

                if (lastHeight > Mathf.Epsilon)
                {
                    if (_directionLeft || _directionDown)
                        lastHeight *= -1.0f;

                    foreach (ScrollableElement element in scrollableElements)
                    {
                        element.parentRect.position += (Vector3)(_directionHorizontal ?
                            Vector2.right * lastHeight :
                            Vector2.up * lastHeight);
                    }
                }
            }
        }
    }

    int GetTargetElementCount(int value)
    {
        if (value - _rows == _maxElementCount && !_infinite)
            return _maxElementCount + 1 * _rows;
        else if (value > _maxElementCount)
            return _maxElementCount + 2 * _rows;
        else
            return value;
    }

    void EnsureElementCount()
    {
        int targetCount = GetTargetElementCount(_elementCount);
        int diff = targetCount - scrollableElements.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; ++i)
            {
                GameObject scrollableGameObject = Instantiate(_scrollElementPrefab, _parentRect);
                ScrollableElement scrollableElement = scrollableGameObject.GetComponent<ScrollableElement>();
                SetElementPivotAnchor(scrollableElement.parentRect);

                if (_directionRight)
                {
                    scrollableElement.parentRect.position = new Vector2(
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.x + (scrollableElements.Count % _rows == 0 ? 1 : 0) * _rectWidth,
                        _parentTop - (scrollableElements.Count % _rows) * _rectHeight);
                }
                else if (_directionLeft)
                {
                    scrollableElement.parentRect.position = new Vector2(
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.x - (scrollableElements.Count % _rows == 0 ? 1 : 0) * _rectWidth,
                        _parentTop - (scrollableElements.Count % _rows) * _rectHeight);
                }
                else if (_directionUp)
                {
                    scrollableElement.parentRect.position = new Vector2(
                        _parentLeft + (scrollableElements.Count % _rows) * _rectWidth,
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.y + (scrollableElements.Count % _rows == 0 ? 1 : 0) * _rectHeight);
                }
                else if (_directionDown)
                {
                    scrollableElement.parentRect.position = new Vector2(
                        _parentLeft + (scrollableElements.Count % _rows) * _rectWidth,
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.y - (scrollableElements.Count % _rows == 0 ? 1 : 0) * _rectHeight);
                }

                scrollableElements.Add(scrollableElement);
            }
        }
        else
        {
            for (int i = 0, count = -diff; i < count; ++i)
            {
                int index = scrollableElements.Count - 1;
                Destroy(scrollableElements[index].gameObject);
                scrollableElements.RemoveAt(index);
            }
        }

        int dataCount = _scrollData.Count;

        if (_scrollable && _infinite)
        {
            for (int i = 0, count = scrollableElements.Count; i < count; ++i)
                scrollableElements[i].gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0, count = scrollableElements.Count; i < count; ++i)
                scrollableElements[i].gameObject.SetActive(i < dataCount);
        }
    }

    protected override void PointerDownEvent(PointerEventData eventData)
    {
        _startClickPos = eventData.position * CanvasScaleManager.screenScalerRatio;
        _lastClickPos = _directionHorizontal ? _startClickPos.x : _startClickPos.y;
        _startedDragging = false;
        _clickDifference = 0.0f;
    }

    protected override void PointerUpEvent(PointerEventData eventData)
    {
        float newClickPosition = _directionHorizontal ?
                eventData.position.x * CanvasScaleManager.screenScalerRatio.x :
                eventData.position.y * CanvasScaleManager.screenScalerRatio.y;

        if (_startedDragging)
        {
            _clickDifference += (newClickPosition - _lastClickPos) * _scrollerParameters.scrollSensitivity;
            _lastClickPos = newClickPosition;
            return;
        }

        if (Time.time < _scrollStartTime)
            return;

        if (Mathf.Abs(newClickPosition - (_directionHorizontal ? _startClickPos.x : _startClickPos.y)) > _scrollerParameters.dragAmountBeforeScroll)
            return;

        for (int i = 0, count = scrollableElements.Count; i < count; ++i)
        {
            if (scrollableElements[i].gameObject.activeInHierarchy && scrollableElements[i].parentRect.rect.Contains((
                eventData.position -
                new Vector2(scrollableElements[i].parentRect.position.x, scrollableElements[i].parentRect.position.y)
                ) * CanvasScaleManager.screenScalerRatio))
            {
                scrollableElements[i].OnClick(eventData.position);
                break;
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _clickDifference = 0.0f;
    }

    void Update()
    {
        if (_elementCount < 1)
            return;

        if (!_scrollable)
        {
            ReturnElementsToStart();
            TurnOffScroller();
            return;
        }

        TurnOnScroller();
        UpdateScrollDifference();

        if (!BounceRectsBack() && Mathf.Abs(_clickDifference) > Mathf.Epsilon)
        {
            Scroll();
            SwapElements();
            UpdateScrollerPosition();
        }
    }

    void ReturnElementsToStart()
    {
        float offset = _directionHorizontal ?
            scrollableElements[0].parentRect.anchoredPosition.x :
            scrollableElements[0].parentRect.anchoredPosition.y;

        if (offset > Mathf.Epsilon)
        {
            UIHelpers.GetClampedLerpPosition(offset, 0.0f, _scrollerParameters.minimumScrollSpeed, _scrollerParameters.scrollSpeed, out float newOffset);
            float diff = newOffset - offset;

            foreach (ScrollableElement element in scrollableElements)
            {
                element.parentRect.anchoredPosition += _directionHorizontal ?
                    Vector2.right * diff :
                    Vector2.up * diff;
            }
        }
    }

    void UpdateScrollDifference()
    {
        if (!buttonDown || Time.time < _scrollStartTime)
            return;

        float newClickPosition = _directionHorizontal ?
            eventData.position.x * CanvasScaleManager.screenScalerRatio.x :
            eventData.position.y * CanvasScaleManager.screenScalerRatio.y;

        if (_startedDragging)
        {
            _clickDifference += (newClickPosition - _lastClickPos) * _scrollerParameters.scrollSensitivity;
            _lastClickPos = newClickPosition;
        }
        else if (Mathf.Abs(newClickPosition - (_directionHorizontal ? _startClickPos.x : _startClickPos.y)) > _scrollerParameters.dragAmountBeforeScroll)
        {
            _startedDragging = true;
            _clickDifference = (newClickPosition - (_directionHorizontal ? _startClickPos.x : _startClickPos.y)) * _scrollerParameters.scrollSensitivity;
            _lastClickPos = newClickPosition;
        }
        else
            _clickDifference = 0.0f;
    }

    bool BounceRectsBack()
    {
        if (_infinite)
            return false;

        float clickDirection = 1.0f;

        if (_directionRight)
            clickDirection = -1.0f;
        else if (_directionLeft)
            clickDirection = 1.0f;
        else if (_directionUp)
            clickDirection = -1.0f;
        else if (_directionDown)
            clickDirection = 1.0f;

        if (_indexOffset == 0 && (Mathf.Abs(_clickDifference) < Mathf.Epsilon || !Mathf.Approximately(Mathf.Sign(_clickDifference), clickDirection)))
        {
            float firstHeight = 0.0f;

            if (_directionRight)
                firstHeight = scrollableElements[0].parentRect.position.x - _parentLeft;
            else if (_directionLeft)
                firstHeight = _parentRight - scrollableElements[0].parentRect.position.x;
            else if (_directionUp)
                firstHeight = scrollableElements[0].parentRect.position.y - _parentBottom;
            else if (_directionDown)
                firstHeight = _parentTop - scrollableElements[0].parentRect.position.y;

            if (firstHeight > Mathf.Epsilon)
            {
                Bounce(firstHeight * clickDirection);
                UpdateScrollerPosition();
                return true;
            }
        }

        int lastIndexOffset = Mathf.Max(_elementCount - scrollableElements.Count, 0);

        if (_indexOffset >= lastIndexOffset && (Mathf.Abs(_clickDifference) < Mathf.Epsilon || Mathf.Approximately(Mathf.Sign(_clickDifference), clickDirection)))
        {
            float lastHeight = 0.0f;

            if (_directionRight)
                lastHeight = _parentRight - (scrollableElements[scrollableElements.Count - 1].parentRect.position.x + _rectWidth);
            else if (_directionLeft)
                lastHeight = (scrollableElements[scrollableElements.Count - 1].parentRect.position.x - _rectWidth) - _parentLeft;
            else if (_directionUp)
                lastHeight = _parentTop - (scrollableElements[scrollableElements.Count - 1].parentRect.position.y + _rectHeight);
            else if (_directionDown)
                lastHeight = (scrollableElements[scrollableElements.Count - 1].parentRect.position.y - _rectHeight) - _parentBottom;

            if (lastHeight > Mathf.Epsilon)
            {
                Bounce(-lastHeight * clickDirection);
                return true;
            }
        }
        return false;
    }

    void Bounce(float offset)
    {
        UIHelpers.GetClampedLerpPosition(_clickDifference, 0.0f, _scrollerParameters.minimumScrollSpeed, _scrollerParameters.scrollSpeed, out float newDiff);
        _clickDifference = newDiff;

        UIHelpers.GetClampedLerpPosition(offset, 0.0f, _scrollerParameters.minimumScrollSpeed, _scrollerParameters.scrollSpeed, out float newOffset);
        float diff = offset - newOffset + newDiff * Time.deltaTime;

        foreach (ScrollableElement element in scrollableElements)
        {
            element.parentRect.anchoredPosition += _directionHorizontal ?
                    Vector2.right * diff :
                    Vector2.up * diff;
        }
    }

    void Scroll()
    {
        UIHelpers.GetClampedLerpPosition(0.0f, _clickDifference, _scrollerParameters.minimumScrollSpeed, _scrollerParameters.scrollSpeed, out float newOffset);
        _clickDifference -= newOffset;

        foreach (ScrollableElement element in scrollableElements)
        {
            element.parentRect.anchoredPosition += _directionHorizontal ?
                Vector2.right * newOffset :
                Vector2.up * newOffset;
        }
    }

    void SwapElements()
    {
        float firstDifference = 0.0f;

        if (_directionRight)
            firstDifference = scrollableElements[0].parentRect.position.x - _parentLeft;
        else if (_directionLeft)
            firstDifference = _parentRight - scrollableElements[0].parentRect.position.x;
        else if (_directionUp)
            firstDifference = scrollableElements[0].parentRect.position.y - _parentBottom;
        else if (_directionDown)
            firstDifference = _parentTop - scrollableElements[0].parentRect.position.y;

        if (firstDifference > Mathf.Epsilon)
        {
            if (_infinite || _indexOffset > 0)
            {
                float diff = firstDifference / (_directionHorizontal ? _rectWidth : _rectHeight);
                int count = (Mathf.FloorToInt(diff) + 1) * _rows;
                SwapLastToFirst(count);
            }
            return;
        }

        float lastDifference = 0.0f;

        if (_directionRight)
            lastDifference = _parentRight - (scrollableElements[scrollableElements.Count - 1].parentRect.position.x + _rectWidth);
        else if (_directionLeft)
            lastDifference = (scrollableElements[scrollableElements.Count - 1].parentRect.position.x - _rectWidth) - _parentLeft;
        else if (_directionUp)
            lastDifference = _parentTop - (scrollableElements[scrollableElements.Count - 1].parentRect.position.y + _rectHeight);
        else if (_directionDown)
            lastDifference = (scrollableElements[scrollableElements.Count - 1].parentRect.position.y - _rectHeight) - _parentBottom;

        if (lastDifference > Mathf.Epsilon)
        {
            int lastIndexOffset = Mathf.Max(_elementCount - scrollableElements.Count, 0);

            if (_infinite || _indexOffset < lastIndexOffset)
            {
                float diff = lastDifference / (_directionHorizontal ? _rectWidth : _rectHeight);
                int count = (Mathf.FloorToInt(diff) + 1) * _rows;
                SwapFirstToLast(count);
            }
            return;
        }
    }

    void SwapLastToFirst(int count) => Swap(count, true/*To The Begining*/);

    void SwapFirstToLast(int count) => Swap(count, false/*To The End*/);

    void Swap(int count, bool toBegining)
    {
        if (!_infinite)
        {
            if (toBegining)
            {
                if (count > _indexOffset)
                    count = _indexOffset;
            }
            else
            {
                int lastIndexOffset = Mathf.Max(_elementCount - scrollableElements.Count, 0);

                if (_indexOffset + count > lastIndexOffset)
                    count = lastIndexOffset - _indexOffset;
            }
        }

        _indexOffset -= toBegining ? count : -count;
        ScrollableElement element = null;

        for (int i = 0; i < count; ++i)
        {
            if (toBegining)
            {
                element = scrollableElements[scrollableElements.Count - 1];
                scrollableElements.RemoveAt(scrollableElements.Count - 1);

                if (_directionRight)
                {
                    element.parentRect.position = new Vector2(
                        scrollableElements[0].parentRect.position.x - (i % _rows == 0 ? 1 : 0) * _rectWidth,
                        _parentTop - ((_rows - 1) - (i % _rows)) * _rectHeight);
                }
                else if (_directionLeft)
                {
                    element.parentRect.position = new Vector2(
                        scrollableElements[0].parentRect.position.x + (i % _rows == 0 ? 1 : 0) * _rectWidth,
                        _parentTop - ((_rows - 1) - (i % _rows)) * _rectHeight);
                }
                else if (_directionUp)
                {
                    element.parentRect.position = new Vector2(
                        _parentLeft + ((_rows - 1) - (i % _rows)) * _rectWidth,
                        scrollableElements[0].parentRect.position.y - (i % _rows == 0 ? 1 : 0) * _rectHeight);
                }
                else if (_directionDown)
                {
                    element.parentRect.position = new Vector2(
                        _parentLeft + ((_rows - 1) - (i % _rows)) * _rectWidth,
                        scrollableElements[0].parentRect.position.y + (i % _rows == 0 ? 1 : 0) * _rectHeight);
                }

                scrollableElements.Insert(0, element);
            }
            else
            {
                element = scrollableElements[0];
                scrollableElements.RemoveAt(0);

                if (_directionRight)
                {
                    element.parentRect.position = new Vector2(
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.x + (i % _rows == 0 ? 1 : 0) * _rectWidth,
                        _parentTop - (i % _rows) * _rectHeight);
                }
                else if (_directionLeft)
                {
                    element.parentRect.position = new Vector2(
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.x - (i % _rows == 0 ? 1 : 0) * _rectWidth,
                        _parentTop - (i % _rows) * _rectHeight);
                }
                else if (_directionUp)
                {
                    element.parentRect.position = new Vector2(
                        _parentLeft + (i % _rows) * _rectWidth,
                        scrollableElements[scrollableElements.Count - 1].parentRect.position.y + (i % _rows == 0 ? 1 : 0) * _rectHeight);
                }
                else if (_directionDown)
                {
                    element.parentRect.position = new Vector2(
                    _parentLeft + (i % _rows) * _rectWidth,
                    scrollableElements[scrollableElements.Count - 1].parentRect.position.y - (i % _rows == 0 ? 1 : 0) * _rectHeight);
                }

                scrollableElements.Add(element);
            }

            if (_infinite)
            {
                int finalOffset = 0;

                if (toBegining)
                {
                    finalOffset = (_indexOffset + (count - 1 - i)) % _scrollData.Count;

                    if (finalOffset < 0)
                        finalOffset = _scrollData.Count + finalOffset;
                }
                else
                {
                    finalOffset = (scrollableElements.Count - 1 + _indexOffset - (count - 1 - i)) % _scrollData.Count;

                    if (finalOffset < 0)
                        finalOffset = _scrollData.Count + finalOffset;
                }

                element.SetContent(_scrollData[finalOffset]);
                element.SetAppeared();
            }
            else
            {
                int finalIndex = 0;

                if (toBegining)
                    finalIndex = _indexOffset + (count - 1 - i);
                else
                    finalIndex = scrollableElements.Count - 1 + _indexOffset - (count - 1 - i);

                // This is used when you have some empty elements in a row
                if (finalIndex >= _scrollData.Count)
                    element.gameObject.SetActive(false);
                else
                {
                    element.SetContent(_scrollData[finalIndex]);
                    element.gameObject.SetActive(true);
                    element.SetAppeared();
                }
            }
        }
    }

    void TurnOffScroller()
    {
        if (_scroller)
            _scroller.gameObject.SetActive(false);

        if (_scrollerParent)
            _scrollerParent.gameObject.SetActive(false);
    }

    void TurnOnScroller()
    {
        if (_scroller)
            _scroller.gameObject.SetActive(true);

        if (_scrollerParent)
            _scrollerParent.gameObject.SetActive(true);
    }

    void UpdateScrollerPosition()
    {
        if (_scroller == null || _scrollerParent == null)
            return;

        float scrollerSize = 0.0f;
        float panelSize = 0.0f;
        float screenSize = 0.0f;
        Vector2 size = _scroller.sizeDelta;
        int collumnCount = Mathf.CeilToInt(_elementCount / (float)_rows);

        if (_directionHorizontal)
        {
            float left = UIHelpers.GetRectLeft(_parentRect);
            float right = UIHelpers.GetRectRight(_parentRect);

            panelSize = _rectWidth * collumnCount;
            screenSize = right - left;
            scrollerSize = screenSize / panelSize;
        }
        else
        {
            float top = UIHelpers.GetRectTop(_parentRect);
            float bottom = UIHelpers.GetRectBottom(_parentRect);

            panelSize = _rectHeight * collumnCount;
            screenSize = top - bottom;
            scrollerSize = screenSize / panelSize;
        }

        float overflowSize = 0.0f;
        float positionMax = 0.0f;

        if (scrollerSize >= 1.0f)
        {
            scrollerSize = 1.0f;
            _scroller.anchoredPosition = Vector2.zero;
        }
        else
        {
            overflowSize = panelSize - screenSize;

            if (_directionHorizontal)
                positionMax = _scrollerParent.sizeDelta.x * (1.0f - scrollerSize);
            else
                positionMax = _scrollerParent.sizeDelta.y * (1.0f - scrollerSize);
        }

        if (_directionHorizontal)
            size.x = scrollerSize * _scrollerParent.sizeDelta.x;
        else
            size.y = scrollerSize * _scrollerParent.sizeDelta.y;

        _scroller.sizeDelta = size;

        if (scrollerSize >= 1.0f)
        {
            _scroller.anchoredPosition = Vector2.zero;
            return;
        }

        int lastIndex = _indexOffset + scrollableElements.Count;
        int currentCollumn = Mathf.CeilToInt(lastIndex / (float)_rows);
        int collumnsLeft = collumnCount - currentCollumn;
        float sizeLeft = 0.0f;

        if (_directionDown)
        {
            float lastElementBottom = _parentBottom - UIHelpers.GetRectBottom(scrollableElements[scrollableElements.Count - 1].parentRect);
            sizeLeft = collumnsLeft * _rectHeight + lastElementBottom;
        }
        else if (_directionUp)
        {
            float lastElementTop = UIHelpers.GetRectTop(scrollableElements[scrollableElements.Count - 1].parentRect) - _parentTop;
            sizeLeft = collumnsLeft * _rectHeight + lastElementTop;
        }
        else if (_directionRight)
        {
            float lastElementRight = UIHelpers.GetRectRight(scrollableElements[scrollableElements.Count - 1].parentRect) - _parentRight;
            sizeLeft = collumnsLeft * _rectWidth + lastElementRight;
        }
        else
        {
            float lastElementLeft = _parentLeft - UIHelpers.GetRectLeft(scrollableElements[scrollableElements.Count - 1].parentRect);
            sizeLeft = collumnsLeft * _rectWidth + lastElementLeft;
        }

        float progress = 1.0f - (sizeLeft / overflowSize);

        if (progress < 0.0f)
            progress = 0.0f;
        else if (progress > 1.0f)
            progress = 1.0f;

        if (_directionDown)
            _scroller.anchoredPosition = new Vector2(0.0f, -positionMax * progress);
        else if (_directionUp)
            _scroller.anchoredPosition = new Vector2(0.0f, -positionMax * (1.0f - progress));
        else if (_directionRight)
            _scroller.anchoredPosition = new Vector2(positionMax * progress, 0.0f);
        else
            _scroller.anchoredPosition = new Vector2(positionMax * (1.0f - progress), 0.0f);
    }
}