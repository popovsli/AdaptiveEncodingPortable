using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AdaptiveEncodingPortable.Configuration
{
    public sealed class MyCompressionMessageEncodingBindingElement : MessageEncodingBindingElement //BindingElement
    {

        //We will use an inner binding element to store information required for the inner encoder
        MessageEncodingBindingElement innerBindingElement;

        CompressionAlgorithm compressionAlgorithm;

        //By default, use the default text encoder as the inner encoder
        public MyCompressionMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement(), CompressionAlgorithm.GZip) { }

        public MyCompressionMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement, CompressionAlgorithm compressionAlgorithm)
        {
            this.innerBindingElement = messageEncoderBindingElement;
            this.compressionAlgorithm = compressionAlgorithm;
        }

        public MessageEncodingBindingElement InnerMessageEncodingBindingElement
        {
            get { return innerBindingElement; }
            set { innerBindingElement = value; }
        }

        public CompressionAlgorithm CompressionAlgorithm
        {
            get { return this.compressionAlgorithm; }
            set { this.compressionAlgorithm = value; }
        }

        //Main entry point into the encoder binding element. Called by WCF to get the factory that will create the
        //message encoder
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new MyCompressionMessageEncoderFactory(innerBindingElement.CreateMessageEncoderFactory(), this.compressionAlgorithm);
        }

        public override MessageVersion MessageVersion
        {
            get { return innerBindingElement.MessageVersion; }
            set { innerBindingElement.MessageVersion = value; }
        }

        public override BindingElement Clone()
        {
            return new MyCompressionMessageEncodingBindingElement(this.innerBindingElement, this.compressionAlgorithm);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return innerBindingElement.GetProperty<T>(context);
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        //public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        //{
        //    if (context == null)
        //        throw new ArgumentNullException("context");

        //    context.BindingParameters.Add(this);
        //    return context.BuildInnerChannelListener<TChannel>();
        //}

        //public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        //{
        //    if (context == null)
        //        throw new ArgumentNullException("context");

        //    context.BindingParameters.Add(this);
        //    return context.CanBuildInnerChannelListener<TChannel>();
        //}

       
    }
}
