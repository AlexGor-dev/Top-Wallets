#pragma once

#include "WalletV3.h"

namespace ton
{
	class WalletV4 : public WalletV3
	{
	public:
		explicit WalletV4(State state) : WalletV3(std::move(state))
		{
		}

		static td::Ref<vm::Cell> get_init_state(const td::Ed25519::PublicKey& public_key, td::uint32 wallet_id, td::int32 revision) noexcept
		{
			auto code = SmartContractCode::get_code(ton::WalletType::WalletV4, revision);
			auto data = vm::CellBuilder()
				.store_long(0, 32)//seqno
				.store_long(wallet_id, 32)
				.store_bytes(public_key.as_octet_string())
				.store_long(0, 1)
				.finalize();
			return GenericAccount::get_init_state(std::move(code), std::move(data));
		}

		static td::Ref<vm::Cell> make_a_gift_message(const td::Ed25519::PrivateKey& private_key, td::uint32 wallet_id, td::uint32 seqno, td::uint32 valid_until, td::Span<Gift> gifts) noexcept
		{
			CHECK(gifts.size() <= max_gifts_size);

			vm::CellBuilder cb;
			cb.store_long(wallet_id, 32).store_long(valid_until, 32).store_long(seqno, 32);

			cb.store_long(0, 8);//op = 0

			for (auto& gift : gifts)
			{
				td::int32 send_mode = 3;
				if (gift.gramms == -1)
					send_mode += 128;
				cb.store_long(send_mode, 8).store_ref(create_int_message(gift));
			}

			auto message_outer = cb.finalize();
			auto signature = private_key.sign(message_outer->get_hash().as_slice()).move_as_ok();
			return vm::CellBuilder().store_bytes(signature).append_cellslice(vm::load_cell_slice(message_outer)).finalize();
		}

		td::Result<td::Ref<vm::Cell>> make_a_gift_message(const td::Ed25519::PrivateKey& private_key, td::uint32 valid_until, td::Span<Gift> gifts) const override
		{
			TRY_RESULT(seqno, get_seqno());
			TRY_RESULT(wallet_id, get_wallet_id());
			return make_a_gift_message(private_key, wallet_id, seqno, valid_until, gifts);
		}

	};
}