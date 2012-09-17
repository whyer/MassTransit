// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Logging;
    using Magnum.Extensions;
    using RabbitMQ.Client;

    public class RabbitMqConnection :
        Connection
    {
        static readonly ILog _log = Logger.Get(typeof (RabbitMqConnection));
        readonly ConnectionFactory _connectionFactory;
        IConnection _connection;
        bool _disposed;

        public RabbitMqConnection(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IConnection Connection
        {
            get { return _connection; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Connect()
        {
            Disconnect();

            _connection = _connectionFactory.CreateConnection();
        }

        public void Disconnect()
        {
            try
            {
                if (_connection != null)
                {
                    try
                    {
                        if (_connection.IsOpen)
                            _connection.Close(200, "disconnected");
                    }
                    catch (Exception ex)
                    {
                        _log.Warn("Exception while closing RabbitMQ connection", ex);
                    }

                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.Warn("Exception disposing of RabbitMQ connection", ex);
            }
            finally
            {
                _connection = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_disposed)
                throw new ObjectDisposedException("RabbitMqConnection for {0}".FormatWith(_connectionFactory.GetUri()),
                    "Cannot dispose a connection twice");

            try
            {
                Disconnect();
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}