using System;
using System.IO;
using log4net;
using Modbus.Message;

namespace Modbus.IO
{
	/// <summary>
	/// Modbus transport.
	/// </summary>
	public abstract class ModbusTransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(ModbusTransport));
		private int _retries = Modbus.DefaultRetries;

		/// <summary>
		/// Number of times to retry sending message.
		/// </summary>
		public int Retries
		{
			get { return _retries; }
			set { _retries = value; }
		}		

		internal virtual T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new()
		{
			T response = default(T);
			int attempt = 1;
			bool success = false;

			do
			{
				try
				{
					Write(message);
					response = ReadResponse<T>();
					ValidateResponse(message, response);
					success = true;
				}
				catch (TimeoutException te)
				{
					_log.ErrorFormat("Timeout, {0} retries remaining - {1}", _retries + 1 - attempt, te.Message);

					if (attempt++ > _retries)
						throw te;
				}
				catch (IOException ioe)
				{
					_log.ErrorFormat("IO Exception, {0} retries remaining - {1}", _retries + 1 - attempt, ioe.Message);

					if (attempt++ > _retries)
						throw ioe;
				}
				catch (SlaveException se)
				{
					_log.ErrorFormat("Slave Exception, {0} retries remaining - {1}", _retries + 1 - attempt, se.Message);

					if (attempt++ > _retries)
						throw se;
				}
		
			} while (!success);

			return response;
		}

		internal virtual T CreateResponse<T>(byte[] frame) where T : IModbusMessage, new()
		{
			byte functionCode = frame[1];

			// check for slave exception response
			if (functionCode > Modbus.ExceptionOffset)
				throw new SlaveException(ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(frame));

			// create message from frame
			T response = ModbusMessageFactory.CreateModbusMessage<T>(frame);

			return response;
		}

		internal virtual void ValidateResponse(IModbusMessage request, IModbusMessage response)
		{
			if (request.FunctionCode != response.FunctionCode)
				throw new IOException(String.Format("Received response with unexpected Function Code. Expected {0}, received {1}.", request.FunctionCode, response.FunctionCode));
		}

		internal abstract byte[] ReadRequest();
		internal abstract T ReadResponse<T>() where T : IModbusMessage, new();
		internal abstract byte[] BuildMessageFrame(IModbusMessage message);		
		internal abstract void Write(IModbusMessage message);
	}
}