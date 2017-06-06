using TMoonEventSystem;

public class BindableProperty<T>
{
    private string type;

    private T _value;
    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (!object.Equals(_value, value))
            {
                T old = _value;
                _value = value;
                Message m = Message.Allocate();
                m.Data = value;
                m.Type = type;
                MessageDispatcher.SendMessage(m);
                Message.Release(m);
            }
        }
    }

    public BindableProperty<T> SetType(string type)
    {
        this.type = type;
        return this;
    }
    public override string ToString()
    {
        return (Value != null ? Value.ToString() : "null");
    }
}