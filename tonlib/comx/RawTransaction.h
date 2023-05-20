#pragma once

#include "crypto/vm/cells/CellString.h"
#include "tonlib/TonlibError.h"
#include "tonlib/keys/SimpleEncryption.h"
//#include "headers.h"

namespace comx
{
	enum class MessageType
	{
		None,
		Text,
		Raw,
		DecryptedText,
		EncryptedText
	};
	class internal_transactionId final : public Object
	{
	public:
		std::int64_t lt_;
		std::string hash_;
		internal_transactionId()
			: lt_()
			, hash_()
		{
		}

		internal_transactionId(std::int64_t lt_, std::string const& hash_)
			: lt_(lt_)
			, hash_(std::move(hash_))
		{
		}
	};



	class accountAddress final : public Object
	{
	public:
		std::string account_address_;
		accountAddress()
			: account_address_()
		{
		}

		accountAddress(std::string const& account_address_)
			: account_address_(std::move(account_address_))
		{
		}
	};

	class msg_Data
	{
	public:
		std::string text_;
		msg_Data()
			: text_()
		{
		}

		msg_Data(std::string const& text_)
			: text_(std::move(text_))
		{
		}

		virtual MessageType getType()
		{
			return MessageType::None;
		}
	};

	class msg_dataRaw final : public msg_Data
	{
	public:
		std::string body_;
		std::string init_state_;
		msg_dataRaw()
			: msg_Data()
			, body_()
			, init_state_()
		{
		}

		msg_dataRaw(std::string const& body_, std::string const& init_state_)
			: msg_Data(""), body_(std::move(body_))
			, init_state_(std::move(init_state_))
		{
		}
		MessageType getType() override
		{
			return MessageType::Raw;
		}
	};

	class msg_dataText final : public msg_Data
	{
	public:
		msg_dataText()
			: msg_Data()
		{
		}

		msg_dataText(std::string const& text_)
			: msg_Data(std::move(text_))
		{
		}
		MessageType getType() override
		{
			return MessageType::Text;
		}

	};

	class msg_dataDecryptedText final : public msg_Data
	{
	public:
		msg_dataDecryptedText()
			: msg_Data()
		{
		}

		msg_dataDecryptedText(std::string const& text_)
			: msg_Data(std::move(text_))
		{
		}
		MessageType getType() override
		{
			return MessageType::DecryptedText;
		}

	};

	class msg_dataEncryptedText final : public msg_Data
	{
	public:
		msg_dataEncryptedText(std::string const& text_)
			: msg_Data(std::move(text_))
		{
		}
		MessageType getType() override
		{
			return MessageType::EncryptedText;
		}

	};

	class raw_message final : public Object
	{
	public:
		object_ptr<accountAddress> source_;
		object_ptr<accountAddress> destination_;
		std::int64_t value_;
		std::int64_t fwd_fee_;
		std::int64_t ihr_fee_;
		std::int64_t created_lt_;
		std::string body_hash_;
		std::string hash;
		object_ptr<msg_Data> msg_data_;

		raw_message(object_ptr<accountAddress>&& source_, object_ptr<accountAddress>&& destination_, std::string const& hash, std::int64_t value_, std::int64_t fwd_fee_, std::int64_t ihr_fee_, std::int64_t created_lt_, std::string const& body_hash_, object_ptr<msg_Data>&& msg_data_)
			: source_(std::move(source_))
			, destination_(std::move(destination_))
			, hash(std::move(hash))
			, value_(value_)
			, fwd_fee_(fwd_fee_)
			, ihr_fee_(ihr_fee_)
			, created_lt_(created_lt_)
			, body_hash_(std::move(body_hash_))
			, msg_data_(std::move(msg_data_))
		{
		}
	};

	class raw_transaction final : public Object
	{
	public:
		std::int64_t utime_;
		object_ptr<internal_transactionId> transaction_id;
		std::string data_;
		object_ptr<internal_transactionId> prev_transaction_id_;
		std::int64_t fee_;
		std::int64_t storage_fee_;
		std::int64_t other_fee_;
		object_ptr<raw_message> in_msg_;
		std::vector<object_ptr<raw_message>> out_msgs_;

		raw_transaction(std::int64_t utime_, object_ptr<internal_transactionId> transaction_id, std::string const& data_, object_ptr<internal_transactionId>&& prev_transaction_id_, std::int64_t fee_, std::int64_t storage_fee_, std::int64_t other_fee_, object_ptr<raw_message>&& in_msg_, std::vector<object_ptr<raw_message>>&& out_msgs_)
			: utime_(utime_)
			, transaction_id(std::move(transaction_id))
			, data_(std::move(data_))
			, prev_transaction_id_(std::move(prev_transaction_id_))
			, fee_(fee_)
			, storage_fee_(storage_fee_)
			, other_fee_(other_fee_)
			, in_msg_(std::move(in_msg_))
			, out_msgs_(std::move(out_msgs_))
		{
		}
	};

	class raw_transactions final : public Object
	{
	public:
		std::vector<object_ptr<raw_transaction>> transactions_;
		object_ptr<internal_transactionId> previous_transaction_id_;
		raw_transactions(std::vector<object_ptr<raw_transaction>>&& transactions_, object_ptr<internal_transactionId>&& previous_transaction_id_)
			: transactions_(std::move(transactions_))
			, previous_transaction_id_(std::move(previous_transaction_id_))
		{
		}
	};

	struct  rawtransaction final : public Object
	{
		int type{ 1 };
		byte hash[32];
		byte msg_hash[32];
		std::int64_t lt;
		std::int64_t utime;
		std::int64_t fee;
		rawtransaction(raw_transaction* trans)
			:lt(trans->transaction_id->lt_), utime(trans->utime_), fee(trans->fee_)
		{
			memcpy(this->hash, trans->transaction_id->hash_.c_str(), 32);
			if (trans->in_msg_)
				memcpy(this->msg_hash, trans->in_msg_->hash.c_str(), 32);
			else
				memset(this->msg_hash, 0, 32);
		}
	};

	struct  rawmessage final : public Object
	{
		int type{ 2 };
		byte source[64];
		byte destination[64];
		std::int64_t value;
		char* msg_data;
		int meaageType;
		int isOut;

		rawmessage(object_ptr<raw_message> message, bool isOut)
			:isOut(isOut), value(message->value_)
		{
			memset(this->source, 0, 64);
			memset(this->destination, 0, 64);
			this->msg_data = nullptr;

			std::string text = message->source_.release()->account_address_;
			if (!text.empty())
				memcpy(this->source, text.c_str(), text.size());
			text = message->destination_.release()->account_address_;
			if (!text.empty())
				memcpy(this->destination, text.c_str(), text.size());

			this->meaageType = (int)message->msg_data_->getType();
			if (message->msg_data_->getType() == MessageType::Raw)
			{
				text = td::base64_encode(((msg_dataRaw*)message->msg_data_.get())->body_);
				this->msg_data = new char[text.size() + 1];
				strcpy(msg_data, text.c_str());
			}
			else
			{
				text = message->msg_data_->text_;
				this->msg_data = new char[text.size() + 1];
				strcpy(msg_data, text.c_str());
			}
		}
		~rawmessage()
		{
			delete[] this->msg_data;
		}

	};
	struct RawTransaction
	{
		byte current_lt_hash[32];
		byte prev_lt_hash[32];
		byte msg_hash[32];
		byte source[64];
		byte destination[64];

		std::int64_t current_lt;
		std::int64_t prev_lt;
		std::int64_t created_lt;
		std::int64_t utime;
		std::int64_t fee;
		std::int64_t value;

		char* msg_data;
		int meaageType;
		//std::int64_t storage_fee;
		//std::int64_t other_fee;
		//std::int64_t fwd_fee;
		//std::int64_t ihr_fee;

		int isOut;

	public:
		RawTransaction(raw_transaction* trans, object_ptr<raw_message> message, bool isOut)
			:utime(trans->utime_), fee(trans->fee_), isOut(isOut), prev_lt(trans->prev_transaction_id_->lt_), value(message->value_), created_lt(message->created_lt_), current_lt(trans->transaction_id->lt_)
		{
			memcpy(this->current_lt_hash, trans->transaction_id->hash_.c_str(), 32);
			memcpy(this->prev_lt_hash, trans->prev_transaction_id_->hash_.c_str(), 32);
			memset(this->source, 0, 64);
			memset(this->destination, 0, 64);
			this->msg_data = nullptr;

			if(isOut && trans->in_msg_)
				memcpy(this->msg_hash, trans->in_msg_->hash.c_str(), 32);

			std::string text = message->source_.release()->account_address_;
			if(!text.empty())
				memcpy(this->source, text.c_str(), text.size());
			text = message->destination_.release()->account_address_;
			if (!text.empty())
				memcpy(this->destination, text.c_str(), text.size());

			this->meaageType = (int)message->msg_data_->getType();
			if (message->msg_data_->getType() == MessageType::Raw)
			{
				text = td::base64_encode(((msg_dataRaw*)message->msg_data_.get())->body_);
				this->msg_data = new char[text.size() + 1];
				strcpy(msg_data, text.c_str());
			}
			else
			{
				text = message->msg_data_->text_;
				this->msg_data = new char[text.size() + 1];
				strcpy(msg_data, text.c_str());

			}
		}
		~RawTransaction()
		{
			delete[] this->msg_data;
		}
	};

	static std::string to_bytes(td::Ref<vm::Cell> cell)
	{
		if (cell.is_null())
			return "";
		return vm::std_boc_serialize(cell, vm::BagOfCells::Mode::WithCRC32C).move_as_ok().as_slice().str();
	}

	static td::Result<td::int64> to_balance_or_throw(td::Ref<vm::CellSlice> balance_ref)
	{
		vm::CellSlice balance_slice = *balance_ref;
		auto balance = block::tlb::t_Grams.as_integer_skip(balance_slice);
		if (balance.is_null())
		{
			return td::Status::Error("Failed to unpack balance");
		}
		auto res = balance->to_long();
		if (res == td::int64(~0ULL << 63))
		{
			return td::Status::Error("Failed to unpack balance (2)");
		}
		return res;
	}
	static td::Result<td::int64> to_balance(td::Ref<vm::CellSlice> balance_ref)
	{
		return TRY_VM(to_balance_or_throw(std::move(balance_ref)));
	}

	static td::Result<std::string> to_std_address_or_throw(td::Ref<vm::CellSlice> cs)
	{
		auto tag = block::gen::MsgAddressInt().get_tag(*cs);
		if (tag < 0)
		{
			return td::Status::Error("Failed to read MsgAddressInt tag");
		}
		if (tag != block::gen::MsgAddressInt::addr_std)
		{
			return "";
		}
		block::gen::MsgAddressInt::Record_addr_std addr;
		if (!::tlb::csr_unpack(cs, addr))
		{
			return td::Status::Error("Failed to unpack MsgAddressInt");
		}
		return block::StdAddress(addr.workchain_id, addr.address).rserialize(true);
	}

	static td::Result<std::string> to_std_address(td::Ref<vm::CellSlice> cs)
	{
		return TRY_VM(to_std_address_or_throw(std::move(cs)));
	}

	struct ToRawTransactions
	{
		explicit ToRawTransactions(td::optional<td::Ed25519::PrivateKey> private_key) : private_key_(std::move(private_key))
		{
		}

		td::optional<td::Ed25519::PrivateKey> private_key_;

		td::Result<object_ptr<raw_message>> to_raw_message_or_throw(td::Ref<vm::Cell> cell)
		{
			auto hash = std::string(cell->get_hash().as_slice().str());
			std::string text = td::base64_encode(cell->get_hash().as_slice());

			block::gen::Message::Record message;
			if (!type_unpack_cell(cell, block::gen::t_Message_Any, message))
			{
				return td::Status::Error("Failed to unpack Message");
			}

			td::Ref<vm::CellSlice> body;
			if (message.body->prefetch_long(1) == 0)
			{
				body = std::move(message.body);
				body.write().advance(1);
			}
			else
			{
				body = vm::load_cell_slice_ref(message.body->prefetch_ref());
			}
			auto body_cell = vm::CellBuilder().append_cellslice(*body).finalize();
			auto body_hash = body_cell->get_hash().as_slice().str();

			auto get_data = [body = std::move(body), body_cell, this](td::Slice salt) mutable
			{
				object_ptr<msg_Data> data;
				if (body->size() >= 32 && static_cast<td::uint32>(body->prefetch_long(32)) <= 1)
				{
					auto type = body.write().fetch_long(32);
					td::Status status;

					auto r_body_message = vm::CellString::load(body.write());
					LOG_IF(WARNING, r_body_message.is_error()) << "Failed to parse a message: " << r_body_message.error();

					if (r_body_message.is_ok())
					{
						if (type == 0)
						{
							data = make_object<msg_dataText>(r_body_message.move_as_ok());
						}
						else
						{
							LOG(ERROR) << "TRY DECRYPT";
							auto encrypted_message = r_body_message.move_as_ok();
							auto r_decrypted_message = [&]() -> td::Result<std::string>
							{
								if (!private_key_)
								{
									return tonlib::TonlibError::EmptyField("private_key");
								}
								TRY_RESULT(decrypted, tonlib::SimpleEncryptionV2::decrypt_data(encrypted_message, private_key_.value(), salt));
								return decrypted.data.as_slice().str();
							}();
							if (r_decrypted_message.is_ok())
							{
								data = make_object<msg_dataDecryptedText>(r_decrypted_message.move_as_ok());
							}
							else
							{
								data = make_object<msg_dataEncryptedText>(encrypted_message);
							}
						}
					}
				}
				if (!data)
				{
					data = make_object<msg_dataRaw>(to_bytes(std::move(body_cell)), "");
				}
				return data;
			};

			auto tag = block::gen::CommonMsgInfo().get_tag(*message.info);
			if (tag < 0)
			{
				return td::Status::Error("Failed to read CommonMsgInfo tag");
			}
			switch (tag)
			{
			case block::gen::CommonMsgInfo::int_msg_info:
			{
				block::gen::CommonMsgInfo::Record_int_msg_info msg_info;
				if (!::tlb::csr_unpack(message.info, msg_info))
				{
					return td::Status::Error("Failed to unpack CommonMsgInfo::int_msg_info");
				}

				TRY_RESULT(balance, to_balance(msg_info.value));
				TRY_RESULT(src, to_std_address(msg_info.src));
				TRY_RESULT(dest, to_std_address(msg_info.dest));
				TRY_RESULT(fwd_fee, to_balance(msg_info.fwd_fee));
				TRY_RESULT(ihr_fee, to_balance(msg_info.ihr_fee));
				auto created_lt = static_cast<td::int64>(msg_info.created_lt);

				return make_object<raw_message>(make_object<accountAddress>(src), make_object<accountAddress>(std::move(dest)), std::move(hash), balance, fwd_fee, ihr_fee, created_lt,
					std::move(body_hash), get_data(src));
			}
			case block::gen::CommonMsgInfo::ext_in_msg_info:
			{
				block::gen::CommonMsgInfo::Record_ext_in_msg_info msg_info;
				if (!::tlb::csr_unpack(message.info, msg_info))
				{
					return td::Status::Error("Failed to unpack CommonMsgInfo::ext_in_msg_info");
				}
				TRY_RESULT(dest, to_std_address(msg_info.dest));
				return make_object<raw_message>(
					make_object<accountAddress>(),
					make_object<accountAddress>(std::move(dest)), std::move(hash), 0, 0, 0, 0, std::move(body_hash),
					get_data(""));
			}
			case block::gen::CommonMsgInfo::ext_out_msg_info:
			{
				block::gen::CommonMsgInfo::Record_ext_out_msg_info msg_info;
				if (!::tlb::csr_unpack(message.info, msg_info))
				{
					return td::Status::Error("Failed to unpack CommonMsgInfo::ext_out_msg_info");
				}
				TRY_RESULT(src, to_std_address(msg_info.src));
				return make_object<raw_message>(
					make_object<accountAddress>(src),
					make_object<accountAddress>(), std::move(hash), 0, 0, 0, 0, std::move(body_hash), get_data(src));
			}
			}

			return td::Status::Error("Unknown CommonMsgInfo tag");
		}

		td::Result<object_ptr<raw_message>> to_raw_message(td::Ref<vm::Cell> cell)
		{
			return TRY_VM(to_raw_message_or_throw(std::move(cell)));
		}

		td::Result<object_ptr<raw_transaction>> to_raw_transaction_or_throw(block::Transaction::Info&& info)
		{
			std::string data;

			object_ptr<raw_message> in_msg;
			std::vector<object_ptr<raw_message>> out_msgs;
			td::int64 fees = 0;
			td::int64 storage_fee = 0;
			if (info.transaction.not_null())
			{
				data = to_bytes(info.transaction);
				block::gen::Transaction::Record trans;
				if (!::tlb::unpack_cell(info.transaction, trans))
				{
					return td::Status::Error("Failed to unpack Transaction");
				}
				TRY_RESULT_ASSIGN(fees, to_balance(trans.total_fees));
				//LOG(ERROR) << fees;

				//std::ostringstream outp;
				//block::gen::t_Transaction.print_ref(outp, info.transaction);
				//LOG(INFO) << outp.str();

				auto is_just = trans.r1.in_msg->prefetch_long(1);
				if (is_just == trans.r1.in_msg->fetch_long_eof)
				{
					return td::Status::Error("Failed to parse long");
				}
				if (is_just == -1)
				{
					//                    auto msg = trans.r1.in_msg->prefetch_ref();
					TRY_RESULT(in_msg_copy, to_raw_message(trans.r1.in_msg->prefetch_ref()));
					in_msg = std::move(in_msg_copy);
				}

				if (trans.outmsg_cnt != 0)
				{
					vm::Dictionary dict{ trans.r1.out_msgs, 15 };
					for (int x = 0; x < trans.outmsg_cnt && x < 100; x++)
					{
						TRY_RESULT(out_msg, to_raw_message(dict.lookup_ref(td::BitArray<15>{x})));
						fees += out_msg->fwd_fee_;
						fees += out_msg->ihr_fee_;
						out_msgs.push_back(std::move(out_msg));
					}
				}
				td::RefInt256 storage_fees;
				if (!block::tlb::t_TransactionDescr.get_storage_fees(trans.description, storage_fees))
				{
					return td::Status::Error("Failed to fetch storage fee from transaction");
				}
				storage_fee = storage_fees->to_long();
			}
			return make_object<raw_transaction>(
				info.now, make_object<internal_transactionId>(info.lt, info.hash.as_slice().str()), data,
				make_object<internal_transactionId>(info.prev_trans_lt, info.prev_trans_hash.as_slice().str()),
				fees, storage_fee, fees - storage_fee, std::move(in_msg), std::move(out_msgs));
		}

		td::Result<object_ptr<raw_transaction>> to_raw_transaction(block::Transaction::Info&& info)
		{
			return TRY_VM(to_raw_transaction_or_throw(std::move(info)));
		}

		td::Result<object_ptr<raw_transactions>> to_raw_transactions(block::TransactionList::Info&& info)
		{
			std::vector<object_ptr<raw_transaction>> transactions;
			for (auto& transaction : info.transactions)
			{
				TRY_RESULT(raw_transaction, to_raw_transaction(std::move(transaction)));
				transactions.push_back(std::move(raw_transaction));
			}

			auto transaction_id = make_object<internal_transactionId>(info.lt, info.hash.as_slice().str());
			//for (auto& transaction : transactions)
			//{
			//	std::swap(transaction->prev_transaction_id_, transaction_id);
			//}

			return make_object<raw_transactions>(std::move(transactions), std::move(transaction_id));
		}
	};
}
