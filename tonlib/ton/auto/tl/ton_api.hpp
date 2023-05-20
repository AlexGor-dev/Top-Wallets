#pragma once

#include "ton_api.h"

namespace ton {
namespace ton_api {

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(Object &obj, const T &func) {
  switch (obj.get_id()) {
    case hashable_bool::ID:
      func(static_cast<hashable_bool &>(obj));
      return true;
    case hashable_int32::ID:
      func(static_cast<hashable_int32 &>(obj));
      return true;
    case hashable_int64::ID:
      func(static_cast<hashable_int64 &>(obj));
      return true;
    case hashable_int256::ID:
      func(static_cast<hashable_int256 &>(obj));
      return true;
    case hashable_bytes::ID:
      func(static_cast<hashable_bytes &>(obj));
      return true;
    case hashable_pair::ID:
      func(static_cast<hashable_pair &>(obj));
      return true;
    case hashable_vector::ID:
      func(static_cast<hashable_vector &>(obj));
      return true;
    case hashable_validatorSessionOldRound::ID:
      func(static_cast<hashable_validatorSessionOldRound &>(obj));
      return true;
    case hashable_validatorSessionRoundAttempt::ID:
      func(static_cast<hashable_validatorSessionRoundAttempt &>(obj));
      return true;
    case hashable_validatorSessionRound::ID:
      func(static_cast<hashable_validatorSessionRound &>(obj));
      return true;
    case hashable_blockSignature::ID:
      func(static_cast<hashable_blockSignature &>(obj));
      return true;
    case hashable_sentBlock::ID:
      func(static_cast<hashable_sentBlock &>(obj));
      return true;
    case hashable_sentBlockEmpty::ID:
      func(static_cast<hashable_sentBlockEmpty &>(obj));
      return true;
    case hashable_vote::ID:
      func(static_cast<hashable_vote &>(obj));
      return true;
    case hashable_blockCandidate::ID:
      func(static_cast<hashable_blockCandidate &>(obj));
      return true;
    case hashable_blockVoteCandidate::ID:
      func(static_cast<hashable_blockVoteCandidate &>(obj));
      return true;
    case hashable_blockCandidateAttempt::ID:
      func(static_cast<hashable_blockCandidateAttempt &>(obj));
      return true;
    case hashable_cntVector::ID:
      func(static_cast<hashable_cntVector &>(obj));
      return true;
    case hashable_cntSortedVector::ID:
      func(static_cast<hashable_cntSortedVector &>(obj));
      return true;
    case hashable_validatorSession::ID:
      func(static_cast<hashable_validatorSession &>(obj));
      return true;
    case storage_ok::ID:
      func(static_cast<storage_ok &>(obj));
      return true;
    case pk_unenc::ID:
      func(static_cast<pk_unenc &>(obj));
      return true;
    case pk_ed25519::ID:
      func(static_cast<pk_ed25519 &>(obj));
      return true;
    case pk_aes::ID:
      func(static_cast<pk_aes &>(obj));
      return true;
    case pk_overlay::ID:
      func(static_cast<pk_overlay &>(obj));
      return true;
    case pub_unenc::ID:
      func(static_cast<pub_unenc &>(obj));
      return true;
    case pub_ed25519::ID:
      func(static_cast<pub_ed25519 &>(obj));
      return true;
    case pub_aes::ID:
      func(static_cast<pub_aes &>(obj));
      return true;
    case pub_overlay::ID:
      func(static_cast<pub_overlay &>(obj));
      return true;
    case testObject::ID:
      func(static_cast<testObject &>(obj));
      return true;
    case testString::ID:
      func(static_cast<testString &>(obj));
      return true;
    case testInt::ID:
      func(static_cast<testInt &>(obj));
      return true;
    case testVectorBytes::ID:
      func(static_cast<testVectorBytes &>(obj));
      return true;
    case adnl_address_udp::ID:
      func(static_cast<adnl_address_udp &>(obj));
      return true;
    case adnl_address_udp6::ID:
      func(static_cast<adnl_address_udp6 &>(obj));
      return true;
    case adnl_address_tunnel::ID:
      func(static_cast<adnl_address_tunnel &>(obj));
      return true;
    case adnl_address_reverse::ID:
      func(static_cast<adnl_address_reverse &>(obj));
      return true;
    case adnl_addressList::ID:
      func(static_cast<adnl_addressList &>(obj));
      return true;
    case adnl_message_createChannel::ID:
      func(static_cast<adnl_message_createChannel &>(obj));
      return true;
    case adnl_message_confirmChannel::ID:
      func(static_cast<adnl_message_confirmChannel &>(obj));
      return true;
    case adnl_message_custom::ID:
      func(static_cast<adnl_message_custom &>(obj));
      return true;
    case adnl_message_nop::ID:
      func(static_cast<adnl_message_nop &>(obj));
      return true;
    case adnl_message_reinit::ID:
      func(static_cast<adnl_message_reinit &>(obj));
      return true;
    case adnl_message_query::ID:
      func(static_cast<adnl_message_query &>(obj));
      return true;
    case adnl_message_answer::ID:
      func(static_cast<adnl_message_answer &>(obj));
      return true;
    case adnl_message_part::ID:
      func(static_cast<adnl_message_part &>(obj));
      return true;
    case adnl_node::ID:
      func(static_cast<adnl_node &>(obj));
      return true;
    case adnl_nodes::ID:
      func(static_cast<adnl_nodes &>(obj));
      return true;
    case adnl_packetContents::ID:
      func(static_cast<adnl_packetContents &>(obj));
      return true;
    case adnl_pong::ID:
      func(static_cast<adnl_pong &>(obj));
      return true;
    case adnl_proxy_none::ID:
      func(static_cast<adnl_proxy_none &>(obj));
      return true;
    case adnl_proxy_fast::ID:
      func(static_cast<adnl_proxy_fast &>(obj));
      return true;
    case adnl_proxyControlPacketPing::ID:
      func(static_cast<adnl_proxyControlPacketPing &>(obj));
      return true;
    case adnl_proxyControlPacketPong::ID:
      func(static_cast<adnl_proxyControlPacketPong &>(obj));
      return true;
    case adnl_proxyControlPacketRegister::ID:
      func(static_cast<adnl_proxyControlPacketRegister &>(obj));
      return true;
    case adnl_proxyPacketHeader::ID:
      func(static_cast<adnl_proxyPacketHeader &>(obj));
      return true;
    case adnl_proxyToFastHash::ID:
      func(static_cast<adnl_proxyToFastHash &>(obj));
      return true;
    case adnl_proxyToFast::ID:
      func(static_cast<adnl_proxyToFast &>(obj));
      return true;
    case adnl_tunnelPacketContents::ID:
      func(static_cast<adnl_tunnelPacketContents &>(obj));
      return true;
    case adnl_config_global::ID:
      func(static_cast<adnl_config_global &>(obj));
      return true;
    case adnl_db_node_key::ID:
      func(static_cast<adnl_db_node_key &>(obj));
      return true;
    case adnl_db_node_value::ID:
      func(static_cast<adnl_db_node_value &>(obj));
      return true;
    case adnl_id_short::ID:
      func(static_cast<adnl_id_short &>(obj));
      return true;
    case catchain_block::ID:
      func(static_cast<catchain_block &>(obj));
      return true;
    case catchain_blockNotFound::ID:
      func(static_cast<catchain_blockNotFound &>(obj));
      return true;
    case catchain_blockResult::ID:
      func(static_cast<catchain_blockResult &>(obj));
      return true;
    case catchain_blocks::ID:
      func(static_cast<catchain_blocks &>(obj));
      return true;
    case catchain_difference::ID:
      func(static_cast<catchain_difference &>(obj));
      return true;
    case catchain_differenceFork::ID:
      func(static_cast<catchain_differenceFork &>(obj));
      return true;
    case catchain_firstblock::ID:
      func(static_cast<catchain_firstblock &>(obj));
      return true;
    case catchain_sent::ID:
      func(static_cast<catchain_sent &>(obj));
      return true;
    case catchain_blockUpdate::ID:
      func(static_cast<catchain_blockUpdate &>(obj));
      return true;
    case catchain_block_data::ID:
      func(static_cast<catchain_block_data &>(obj));
      return true;
    case catchain_block_dep::ID:
      func(static_cast<catchain_block_dep &>(obj));
      return true;
    case catchain_block_id::ID:
      func(static_cast<catchain_block_id &>(obj));
      return true;
    case catchain_block_data_badBlock::ID:
      func(static_cast<catchain_block_data_badBlock &>(obj));
      return true;
    case catchain_block_data_fork::ID:
      func(static_cast<catchain_block_data_fork &>(obj));
      return true;
    case catchain_block_data_nop::ID:
      func(static_cast<catchain_block_data_nop &>(obj));
      return true;
    case catchain_config_global::ID:
      func(static_cast<catchain_config_global &>(obj));
      return true;
    case config_global::ID:
      func(static_cast<config_global &>(obj));
      return true;
    case config_local::ID:
      func(static_cast<config_local &>(obj));
      return true;
    case control_config_local::ID:
      func(static_cast<control_config_local &>(obj));
      return true;
    case db_candidate::ID:
      func(static_cast<db_candidate &>(obj));
      return true;
    case db_block_info::ID:
      func(static_cast<db_block_info &>(obj));
      return true;
    case db_block_packedInfo::ID:
      func(static_cast<db_block_packedInfo &>(obj));
      return true;
    case db_block_archivedInfo::ID:
      func(static_cast<db_block_archivedInfo &>(obj));
      return true;
    case db_blockdb_key_lru::ID:
      func(static_cast<db_blockdb_key_lru &>(obj));
      return true;
    case db_blockdb_key_value::ID:
      func(static_cast<db_blockdb_key_value &>(obj));
      return true;
    case db_blockdb_lru::ID:
      func(static_cast<db_blockdb_lru &>(obj));
      return true;
    case db_blockdb_value::ID:
      func(static_cast<db_blockdb_value &>(obj));
      return true;
    case db_candidate_id::ID:
      func(static_cast<db_candidate_id &>(obj));
      return true;
    case db_celldb_value::ID:
      func(static_cast<db_celldb_value &>(obj));
      return true;
    case db_celldb_key_value::ID:
      func(static_cast<db_celldb_key_value &>(obj));
      return true;
    case db_filedb_key_empty::ID:
      func(static_cast<db_filedb_key_empty &>(obj));
      return true;
    case db_filedb_key_blockFile::ID:
      func(static_cast<db_filedb_key_blockFile &>(obj));
      return true;
    case db_filedb_key_zeroStateFile::ID:
      func(static_cast<db_filedb_key_zeroStateFile &>(obj));
      return true;
    case db_filedb_key_persistentStateFile::ID:
      func(static_cast<db_filedb_key_persistentStateFile &>(obj));
      return true;
    case db_filedb_key_proof::ID:
      func(static_cast<db_filedb_key_proof &>(obj));
      return true;
    case db_filedb_key_proofLink::ID:
      func(static_cast<db_filedb_key_proofLink &>(obj));
      return true;
    case db_filedb_key_signatures::ID:
      func(static_cast<db_filedb_key_signatures &>(obj));
      return true;
    case db_filedb_key_candidate::ID:
      func(static_cast<db_filedb_key_candidate &>(obj));
      return true;
    case db_filedb_key_blockInfo::ID:
      func(static_cast<db_filedb_key_blockInfo &>(obj));
      return true;
    case db_filedb_value::ID:
      func(static_cast<db_filedb_value &>(obj));
      return true;
    case db_files_index_key::ID:
      func(static_cast<db_files_index_key &>(obj));
      return true;
    case db_files_package_key::ID:
      func(static_cast<db_files_package_key &>(obj));
      return true;
    case db_files_index_value::ID:
      func(static_cast<db_files_index_value &>(obj));
      return true;
    case db_files_package_firstBlock::ID:
      func(static_cast<db_files_package_firstBlock &>(obj));
      return true;
    case db_files_package_value::ID:
      func(static_cast<db_files_package_value &>(obj));
      return true;
    case db_lt_el_key::ID:
      func(static_cast<db_lt_el_key &>(obj));
      return true;
    case db_lt_desc_key::ID:
      func(static_cast<db_lt_desc_key &>(obj));
      return true;
    case db_lt_shard_key::ID:
      func(static_cast<db_lt_shard_key &>(obj));
      return true;
    case db_lt_status_key::ID:
      func(static_cast<db_lt_status_key &>(obj));
      return true;
    case db_lt_desc_value::ID:
      func(static_cast<db_lt_desc_value &>(obj));
      return true;
    case db_lt_el_value::ID:
      func(static_cast<db_lt_el_value &>(obj));
      return true;
    case db_lt_shard_value::ID:
      func(static_cast<db_lt_shard_value &>(obj));
      return true;
    case db_lt_status_value::ID:
      func(static_cast<db_lt_status_value &>(obj));
      return true;
    case db_root_config::ID:
      func(static_cast<db_root_config &>(obj));
      return true;
    case db_root_dbDescription::ID:
      func(static_cast<db_root_dbDescription &>(obj));
      return true;
    case db_root_key_cellDb::ID:
      func(static_cast<db_root_key_cellDb &>(obj));
      return true;
    case db_root_key_blockDb::ID:
      func(static_cast<db_root_key_blockDb &>(obj));
      return true;
    case db_root_key_config::ID:
      func(static_cast<db_root_key_config &>(obj));
      return true;
    case db_state_asyncSerializer::ID:
      func(static_cast<db_state_asyncSerializer &>(obj));
      return true;
    case db_state_dbVersion::ID:
      func(static_cast<db_state_dbVersion &>(obj));
      return true;
    case db_state_destroyedSessions::ID:
      func(static_cast<db_state_destroyedSessions &>(obj));
      return true;
    case db_state_gcBlockId::ID:
      func(static_cast<db_state_gcBlockId &>(obj));
      return true;
    case db_state_hardforks::ID:
      func(static_cast<db_state_hardforks &>(obj));
      return true;
    case db_state_initBlockId::ID:
      func(static_cast<db_state_initBlockId &>(obj));
      return true;
    case db_state_key_destroyedSessions::ID:
      func(static_cast<db_state_key_destroyedSessions &>(obj));
      return true;
    case db_state_key_initBlockId::ID:
      func(static_cast<db_state_key_initBlockId &>(obj));
      return true;
    case db_state_key_gcBlockId::ID:
      func(static_cast<db_state_key_gcBlockId &>(obj));
      return true;
    case db_state_key_shardClient::ID:
      func(static_cast<db_state_key_shardClient &>(obj));
      return true;
    case db_state_key_asyncSerializer::ID:
      func(static_cast<db_state_key_asyncSerializer &>(obj));
      return true;
    case db_state_key_hardforks::ID:
      func(static_cast<db_state_key_hardforks &>(obj));
      return true;
    case db_state_key_dbVersion::ID:
      func(static_cast<db_state_key_dbVersion &>(obj));
      return true;
    case db_state_shardClient::ID:
      func(static_cast<db_state_shardClient &>(obj));
      return true;
    case dht_key::ID:
      func(static_cast<dht_key &>(obj));
      return true;
    case dht_keyDescription::ID:
      func(static_cast<dht_keyDescription &>(obj));
      return true;
    case dht_message::ID:
      func(static_cast<dht_message &>(obj));
      return true;
    case dht_node::ID:
      func(static_cast<dht_node &>(obj));
      return true;
    case dht_nodes::ID:
      func(static_cast<dht_nodes &>(obj));
      return true;
    case dht_pong::ID:
      func(static_cast<dht_pong &>(obj));
      return true;
    case dht_requestReversePingCont::ID:
      func(static_cast<dht_requestReversePingCont &>(obj));
      return true;
    case dht_clientNotFound::ID:
      func(static_cast<dht_clientNotFound &>(obj));
      return true;
    case dht_reversePingOk::ID:
      func(static_cast<dht_reversePingOk &>(obj));
      return true;
    case dht_stored::ID:
      func(static_cast<dht_stored &>(obj));
      return true;
    case dht_updateRule_signature::ID:
      func(static_cast<dht_updateRule_signature &>(obj));
      return true;
    case dht_updateRule_anybody::ID:
      func(static_cast<dht_updateRule_anybody &>(obj));
      return true;
    case dht_updateRule_overlayNodes::ID:
      func(static_cast<dht_updateRule_overlayNodes &>(obj));
      return true;
    case dht_value::ID:
      func(static_cast<dht_value &>(obj));
      return true;
    case dht_valueNotFound::ID:
      func(static_cast<dht_valueNotFound &>(obj));
      return true;
    case dht_valueFound::ID:
      func(static_cast<dht_valueFound &>(obj));
      return true;
    case dht_config_global::ID:
      func(static_cast<dht_config_global &>(obj));
      return true;
    case dht_config_global_v2::ID:
      func(static_cast<dht_config_global_v2 &>(obj));
      return true;
    case dht_config_local::ID:
      func(static_cast<dht_config_local &>(obj));
      return true;
    case dht_config_random_local::ID:
      func(static_cast<dht_config_random_local &>(obj));
      return true;
    case dht_db_bucket::ID:
      func(static_cast<dht_db_bucket &>(obj));
      return true;
    case dht_db_key_bucket::ID:
      func(static_cast<dht_db_key_bucket &>(obj));
      return true;
    case dummyworkchain0_config_global::ID:
      func(static_cast<dummyworkchain0_config_global &>(obj));
      return true;
    case engine_addr::ID:
      func(static_cast<engine_addr &>(obj));
      return true;
    case engine_addrProxy::ID:
      func(static_cast<engine_addrProxy &>(obj));
      return true;
    case engine_adnl::ID:
      func(static_cast<engine_adnl &>(obj));
      return true;
    case engine_controlInterface::ID:
      func(static_cast<engine_controlInterface &>(obj));
      return true;
    case engine_controlProcess::ID:
      func(static_cast<engine_controlProcess &>(obj));
      return true;
    case engine_dht::ID:
      func(static_cast<engine_dht &>(obj));
      return true;
    case engine_gc::ID:
      func(static_cast<engine_gc &>(obj));
      return true;
    case engine_liteServer::ID:
      func(static_cast<engine_liteServer &>(obj));
      return true;
    case engine_validator::ID:
      func(static_cast<engine_validator &>(obj));
      return true;
    case engine_validatorAdnlAddress::ID:
      func(static_cast<engine_validatorAdnlAddress &>(obj));
      return true;
    case engine_validatorTempKey::ID:
      func(static_cast<engine_validatorTempKey &>(obj));
      return true;
    case engine_adnlProxy_config::ID:
      func(static_cast<engine_adnlProxy_config &>(obj));
      return true;
    case engine_adnlProxy_port::ID:
      func(static_cast<engine_adnlProxy_port &>(obj));
      return true;
    case engine_dht_config::ID:
      func(static_cast<engine_dht_config &>(obj));
      return true;
    case engine_validator_config::ID:
      func(static_cast<engine_validator_config &>(obj));
      return true;
    case engine_validator_controlQueryError::ID:
      func(static_cast<engine_validator_controlQueryError &>(obj));
      return true;
    case engine_validator_dhtServerStatus::ID:
      func(static_cast<engine_validator_dhtServerStatus &>(obj));
      return true;
    case engine_validator_dhtServersStatus::ID:
      func(static_cast<engine_validator_dhtServersStatus &>(obj));
      return true;
    case engine_validator_electionBid::ID:
      func(static_cast<engine_validator_electionBid &>(obj));
      return true;
    case engine_validator_fullNodeMaster::ID:
      func(static_cast<engine_validator_fullNodeMaster &>(obj));
      return true;
    case engine_validator_fullNodeSlave::ID:
      func(static_cast<engine_validator_fullNodeSlave &>(obj));
      return true;
    case validator_groupMember::ID:
      func(static_cast<validator_groupMember &>(obj));
      return true;
    case engine_validator_jsonConfig::ID:
      func(static_cast<engine_validator_jsonConfig &>(obj));
      return true;
    case engine_validator_keyHash::ID:
      func(static_cast<engine_validator_keyHash &>(obj));
      return true;
    case engine_validator_onePerfTimerStat::ID:
      func(static_cast<engine_validator_onePerfTimerStat &>(obj));
      return true;
    case engine_validator_oneStat::ID:
      func(static_cast<engine_validator_oneStat &>(obj));
      return true;
    case engine_validator_overlayStats::ID:
      func(static_cast<engine_validator_overlayStats &>(obj));
      return true;
    case engine_validator_overlayStatsNode::ID:
      func(static_cast<engine_validator_overlayStatsNode &>(obj));
      return true;
    case engine_validator_overlaysStats::ID:
      func(static_cast<engine_validator_overlaysStats &>(obj));
      return true;
    case engine_validator_perfTimerStats::ID:
      func(static_cast<engine_validator_perfTimerStats &>(obj));
      return true;
    case engine_validator_perfTimerStatsByName::ID:
      func(static_cast<engine_validator_perfTimerStatsByName &>(obj));
      return true;
    case engine_validator_proposalVote::ID:
      func(static_cast<engine_validator_proposalVote &>(obj));
      return true;
    case engine_validator_signature::ID:
      func(static_cast<engine_validator_signature &>(obj));
      return true;
    case engine_validator_stats::ID:
      func(static_cast<engine_validator_stats &>(obj));
      return true;
    case engine_validator_success::ID:
      func(static_cast<engine_validator_success &>(obj));
      return true;
    case engine_validator_time::ID:
      func(static_cast<engine_validator_time &>(obj));
      return true;
    case fec_raptorQ::ID:
      func(static_cast<fec_raptorQ &>(obj));
      return true;
    case fec_roundRobin::ID:
      func(static_cast<fec_roundRobin &>(obj));
      return true;
    case fec_online::ID:
      func(static_cast<fec_online &>(obj));
      return true;
    case http_header::ID:
      func(static_cast<http_header &>(obj));
      return true;
    case http_payloadPart::ID:
      func(static_cast<http_payloadPart &>(obj));
      return true;
    case http_response::ID:
      func(static_cast<http_response &>(obj));
      return true;
    case http_proxy_capabilities::ID:
      func(static_cast<http_proxy_capabilities &>(obj));
      return true;
    case http_server_config::ID:
      func(static_cast<http_server_config &>(obj));
      return true;
    case http_server_dnsEntry::ID:
      func(static_cast<http_server_dnsEntry &>(obj));
      return true;
    case http_server_host::ID:
      func(static_cast<http_server_host &>(obj));
      return true;
    case id_config_local::ID:
      func(static_cast<id_config_local &>(obj));
      return true;
    case liteclient_config_global::ID:
      func(static_cast<liteclient_config_global &>(obj));
      return true;
    case liteserver_desc::ID:
      func(static_cast<liteserver_desc &>(obj));
      return true;
    case liteserver_config_local::ID:
      func(static_cast<liteserver_config_local &>(obj));
      return true;
    case liteserver_config_random_local::ID:
      func(static_cast<liteserver_config_random_local &>(obj));
      return true;
    case overlay_fec_received::ID:
      func(static_cast<overlay_fec_received &>(obj));
      return true;
    case overlay_fec_completed::ID:
      func(static_cast<overlay_fec_completed &>(obj));
      return true;
    case overlay_unicast::ID:
      func(static_cast<overlay_unicast &>(obj));
      return true;
    case overlay_broadcast::ID:
      func(static_cast<overlay_broadcast &>(obj));
      return true;
    case overlay_broadcastFec::ID:
      func(static_cast<overlay_broadcastFec &>(obj));
      return true;
    case overlay_broadcastFecShort::ID:
      func(static_cast<overlay_broadcastFecShort &>(obj));
      return true;
    case overlay_broadcastNotFound::ID:
      func(static_cast<overlay_broadcastNotFound &>(obj));
      return true;
    case overlay_broadcastList::ID:
      func(static_cast<overlay_broadcastList &>(obj));
      return true;
    case overlay_certificate::ID:
      func(static_cast<overlay_certificate &>(obj));
      return true;
    case overlay_certificateV2::ID:
      func(static_cast<overlay_certificateV2 &>(obj));
      return true;
    case overlay_emptyCertificate::ID:
      func(static_cast<overlay_emptyCertificate &>(obj));
      return true;
    case overlay_certificateId::ID:
      func(static_cast<overlay_certificateId &>(obj));
      return true;
    case overlay_certificateIdV2::ID:
      func(static_cast<overlay_certificateIdV2 &>(obj));
      return true;
    case overlay_message::ID:
      func(static_cast<overlay_message &>(obj));
      return true;
    case overlay_node::ID:
      func(static_cast<overlay_node &>(obj));
      return true;
    case overlay_nodes::ID:
      func(static_cast<overlay_nodes &>(obj));
      return true;
    case overlay_broadcast_id::ID:
      func(static_cast<overlay_broadcast_id &>(obj));
      return true;
    case overlay_broadcast_toSign::ID:
      func(static_cast<overlay_broadcast_toSign &>(obj));
      return true;
    case overlay_broadcastFec_id::ID:
      func(static_cast<overlay_broadcastFec_id &>(obj));
      return true;
    case overlay_broadcastFec_partId::ID:
      func(static_cast<overlay_broadcastFec_partId &>(obj));
      return true;
    case overlay_db_key_nodes::ID:
      func(static_cast<overlay_db_key_nodes &>(obj));
      return true;
    case overlay_db_nodes::ID:
      func(static_cast<overlay_db_nodes &>(obj));
      return true;
    case overlay_node_toSign::ID:
      func(static_cast<overlay_node_toSign &>(obj));
      return true;
    case rldp_message::ID:
      func(static_cast<rldp_message &>(obj));
      return true;
    case rldp_query::ID:
      func(static_cast<rldp_query &>(obj));
      return true;
    case rldp_answer::ID:
      func(static_cast<rldp_answer &>(obj));
      return true;
    case rldp_messagePart::ID:
      func(static_cast<rldp_messagePart &>(obj));
      return true;
    case rldp_confirm::ID:
      func(static_cast<rldp_confirm &>(obj));
      return true;
    case rldp_complete::ID:
      func(static_cast<rldp_complete &>(obj));
      return true;
    case rldp2_messagePart::ID:
      func(static_cast<rldp2_messagePart &>(obj));
      return true;
    case rldp2_confirm::ID:
      func(static_cast<rldp2_confirm &>(obj));
      return true;
    case rldp2_complete::ID:
      func(static_cast<rldp2_complete &>(obj));
      return true;
    case storage_piece::ID:
      func(static_cast<storage_piece &>(obj));
      return true;
    case storage_pong::ID:
      func(static_cast<storage_pong &>(obj));
      return true;
    case storage_priorityAction_all::ID:
      func(static_cast<storage_priorityAction_all &>(obj));
      return true;
    case storage_priorityAction_idx::ID:
      func(static_cast<storage_priorityAction_idx &>(obj));
      return true;
    case storage_priorityAction_name::ID:
      func(static_cast<storage_priorityAction_name &>(obj));
      return true;
    case storage_state::ID:
      func(static_cast<storage_state &>(obj));
      return true;
    case storage_torrentInfo::ID:
      func(static_cast<storage_torrentInfo &>(obj));
      return true;
    case storage_updateInit::ID:
      func(static_cast<storage_updateInit &>(obj));
      return true;
    case storage_updateHavePieces::ID:
      func(static_cast<storage_updateHavePieces &>(obj));
      return true;
    case storage_updateState::ID:
      func(static_cast<storage_updateState &>(obj));
      return true;
    case storage_daemon_contractInfo::ID:
      func(static_cast<storage_daemon_contractInfo &>(obj));
      return true;
    case storage_daemon_fileInfo::ID:
      func(static_cast<storage_daemon_fileInfo &>(obj));
      return true;
    case storage_daemon_keyHash::ID:
      func(static_cast<storage_daemon_keyHash &>(obj));
      return true;
    case storage_daemon_newContractMessage::ID:
      func(static_cast<storage_daemon_newContractMessage &>(obj));
      return true;
    case storage_daemon_newContractParams::ID:
      func(static_cast<storage_daemon_newContractParams &>(obj));
      return true;
    case storage_daemon_newContractParamsAuto::ID:
      func(static_cast<storage_daemon_newContractParamsAuto &>(obj));
      return true;
    case storage_daemon_peer::ID:
      func(static_cast<storage_daemon_peer &>(obj));
      return true;
    case storage_daemon_peerList::ID:
      func(static_cast<storage_daemon_peerList &>(obj));
      return true;
    case storage_daemon_providerAddress::ID:
      func(static_cast<storage_daemon_providerAddress &>(obj));
      return true;
    case storage_daemon_providerConfig::ID:
      func(static_cast<storage_daemon_providerConfig &>(obj));
      return true;
    case storage_daemon_providerInfo::ID:
      func(static_cast<storage_daemon_providerInfo &>(obj));
      return true;
    case storage_daemon_queryError::ID:
      func(static_cast<storage_daemon_queryError &>(obj));
      return true;
    case storage_daemon_prioritySet::ID:
      func(static_cast<storage_daemon_prioritySet &>(obj));
      return true;
    case storage_daemon_priorityPending::ID:
      func(static_cast<storage_daemon_priorityPending &>(obj));
      return true;
    case storage_daemon_success::ID:
      func(static_cast<storage_daemon_success &>(obj));
      return true;
    case storage_daemon_torrent::ID:
      func(static_cast<storage_daemon_torrent &>(obj));
      return true;
    case storage_daemon_torrentFull::ID:
      func(static_cast<storage_daemon_torrentFull &>(obj));
      return true;
    case storage_daemon_torrentList::ID:
      func(static_cast<storage_daemon_torrentList &>(obj));
      return true;
    case storage_daemon_torrentMeta::ID:
      func(static_cast<storage_daemon_torrentMeta &>(obj));
      return true;
    case storage_daemon_config::ID:
      func(static_cast<storage_daemon_config &>(obj));
      return true;
    case storage_daemon_provider_params::ID:
      func(static_cast<storage_daemon_provider_params &>(obj));
      return true;
    case storage_provider_db_contractAddress::ID:
      func(static_cast<storage_provider_db_contractAddress &>(obj));
      return true;
    case storage_provider_db_contractList::ID:
      func(static_cast<storage_provider_db_contractList &>(obj));
      return true;
    case storage_db_piecesInDb::ID:
      func(static_cast<storage_db_piecesInDb &>(obj));
      return true;
    case storage_db_priorities::ID:
      func(static_cast<storage_db_priorities &>(obj));
      return true;
    case storage_db_torrentList::ID:
      func(static_cast<storage_db_torrentList &>(obj));
      return true;
    case storage_db_torrent::ID:
      func(static_cast<storage_db_torrent &>(obj));
      return true;
    case storage_db_key_pieceInDb::ID:
      func(static_cast<storage_db_key_pieceInDb &>(obj));
      return true;
    case storage_db_key_piecesInDb::ID:
      func(static_cast<storage_db_key_piecesInDb &>(obj));
      return true;
    case storage_db_key_priorities::ID:
      func(static_cast<storage_db_key_priorities &>(obj));
      return true;
    case storage_db_key_torrentList::ID:
      func(static_cast<storage_db_key_torrentList &>(obj));
      return true;
    case storage_db_key_torrentMeta::ID:
      func(static_cast<storage_db_key_torrentMeta &>(obj));
      return true;
    case storage_db_key_torrent::ID:
      func(static_cast<storage_db_key_torrent &>(obj));
      return true;
    case storage_provider_db_microchunkTree::ID:
      func(static_cast<storage_provider_db_microchunkTree &>(obj));
      return true;
    case storage_provider_db_state::ID:
      func(static_cast<storage_provider_db_state &>(obj));
      return true;
    case storage_provider_db_storageContract::ID:
      func(static_cast<storage_provider_db_storageContract &>(obj));
      return true;
    case storage_provider_db_key_contractList::ID:
      func(static_cast<storage_provider_db_key_contractList &>(obj));
      return true;
    case storage_provider_db_key_microchunkTree::ID:
      func(static_cast<storage_provider_db_key_microchunkTree &>(obj));
      return true;
    case storage_provider_db_key_providerConfig::ID:
      func(static_cast<storage_provider_db_key_providerConfig &>(obj));
      return true;
    case storage_provider_db_key_state::ID:
      func(static_cast<storage_provider_db_key_state &>(obj));
      return true;
    case storage_provider_db_key_storageContract::ID:
      func(static_cast<storage_provider_db_key_storageContract &>(obj));
      return true;
    case tcp_authentificate::ID:
      func(static_cast<tcp_authentificate &>(obj));
      return true;
    case tcp_authentificationNonce::ID:
      func(static_cast<tcp_authentificationNonce &>(obj));
      return true;
    case tcp_authentificationComplete::ID:
      func(static_cast<tcp_authentificationComplete &>(obj));
      return true;
    case tcp_pong::ID:
      func(static_cast<tcp_pong &>(obj));
      return true;
    case ton_blockId::ID:
      func(static_cast<ton_blockId &>(obj));
      return true;
    case ton_blockIdApprove::ID:
      func(static_cast<ton_blockIdApprove &>(obj));
      return true;
    case tonNode_archiveNotFound::ID:
      func(static_cast<tonNode_archiveNotFound &>(obj));
      return true;
    case tonNode_archiveInfo::ID:
      func(static_cast<tonNode_archiveInfo &>(obj));
      return true;
    case tonNode_blockDescriptionEmpty::ID:
      func(static_cast<tonNode_blockDescriptionEmpty &>(obj));
      return true;
    case tonNode_blockDescription::ID:
      func(static_cast<tonNode_blockDescription &>(obj));
      return true;
    case tonNode_blockId::ID:
      func(static_cast<tonNode_blockId &>(obj));
      return true;
    case tonNode_blockIdExt::ID:
      func(static_cast<tonNode_blockIdExt &>(obj));
      return true;
    case tonNode_blockSignature::ID:
      func(static_cast<tonNode_blockSignature &>(obj));
      return true;
    case tonNode_blocksDescription::ID:
      func(static_cast<tonNode_blocksDescription &>(obj));
      return true;
    case tonNode_blockBroadcast::ID:
      func(static_cast<tonNode_blockBroadcast &>(obj));
      return true;
    case tonNode_ihrMessageBroadcast::ID:
      func(static_cast<tonNode_ihrMessageBroadcast &>(obj));
      return true;
    case tonNode_externalMessageBroadcast::ID:
      func(static_cast<tonNode_externalMessageBroadcast &>(obj));
      return true;
    case tonNode_newShardBlockBroadcast::ID:
      func(static_cast<tonNode_newShardBlockBroadcast &>(obj));
      return true;
    case tonNode_capabilities::ID:
      func(static_cast<tonNode_capabilities &>(obj));
      return true;
    case tonNode_data::ID:
      func(static_cast<tonNode_data &>(obj));
      return true;
    case tonNode_dataFull::ID:
      func(static_cast<tonNode_dataFull &>(obj));
      return true;
    case tonNode_dataFullEmpty::ID:
      func(static_cast<tonNode_dataFullEmpty &>(obj));
      return true;
    case tonNode_dataList::ID:
      func(static_cast<tonNode_dataList &>(obj));
      return true;
    case tonNode_externalMessage::ID:
      func(static_cast<tonNode_externalMessage &>(obj));
      return true;
    case tonNode_ihrMessage::ID:
      func(static_cast<tonNode_ihrMessage &>(obj));
      return true;
    case tonNode_keyBlocks::ID:
      func(static_cast<tonNode_keyBlocks &>(obj));
      return true;
    case tonNode_newShardBlock::ID:
      func(static_cast<tonNode_newShardBlock &>(obj));
      return true;
    case tonNode_prepared::ID:
      func(static_cast<tonNode_prepared &>(obj));
      return true;
    case tonNode_notFound::ID:
      func(static_cast<tonNode_notFound &>(obj));
      return true;
    case tonNode_preparedProofEmpty::ID:
      func(static_cast<tonNode_preparedProofEmpty &>(obj));
      return true;
    case tonNode_preparedProof::ID:
      func(static_cast<tonNode_preparedProof &>(obj));
      return true;
    case tonNode_preparedProofLink::ID:
      func(static_cast<tonNode_preparedProofLink &>(obj));
      return true;
    case tonNode_preparedState::ID:
      func(static_cast<tonNode_preparedState &>(obj));
      return true;
    case tonNode_notFoundState::ID:
      func(static_cast<tonNode_notFoundState &>(obj));
      return true;
    case tonNode_sessionId::ID:
      func(static_cast<tonNode_sessionId &>(obj));
      return true;
    case tonNode_shardPublicOverlayId::ID:
      func(static_cast<tonNode_shardPublicOverlayId &>(obj));
      return true;
    case tonNode_success::ID:
      func(static_cast<tonNode_success &>(obj));
      return true;
    case tonNode_zeroStateIdExt::ID:
      func(static_cast<tonNode_zeroStateIdExt &>(obj));
      return true;
    case validator_group::ID:
      func(static_cast<validator_group &>(obj));
      return true;
    case validator_groupEx::ID:
      func(static_cast<validator_groupEx &>(obj));
      return true;
    case validator_groupNew::ID:
      func(static_cast<validator_groupNew &>(obj));
      return true;
    case validator_config_global::ID:
      func(static_cast<validator_config_global &>(obj));
      return true;
    case validator_config_local::ID:
      func(static_cast<validator_config_local &>(obj));
      return true;
    case validator_config_random_local::ID:
      func(static_cast<validator_config_random_local &>(obj));
      return true;
    case validatorSession_blockUpdate::ID:
      func(static_cast<validatorSession_blockUpdate &>(obj));
      return true;
    case validatorSession_candidate::ID:
      func(static_cast<validatorSession_candidate &>(obj));
      return true;
    case validatorSession_candidateId::ID:
      func(static_cast<validatorSession_candidateId &>(obj));
      return true;
    case validatorSession_catchainOptions::ID:
      func(static_cast<validatorSession_catchainOptions &>(obj));
      return true;
    case validatorSession_config::ID:
      func(static_cast<validatorSession_config &>(obj));
      return true;
    case validatorSession_configNew::ID:
      func(static_cast<validatorSession_configNew &>(obj));
      return true;
    case validatorSession_configVersioned::ID:
      func(static_cast<validatorSession_configVersioned &>(obj));
      return true;
    case validatorSession_configVersionedV2::ID:
      func(static_cast<validatorSession_configVersionedV2 &>(obj));
      return true;
    case validatorSession_message_startSession::ID:
      func(static_cast<validatorSession_message_startSession &>(obj));
      return true;
    case validatorSession_message_finishSession::ID:
      func(static_cast<validatorSession_message_finishSession &>(obj));
      return true;
    case validatorSession_pong::ID:
      func(static_cast<validatorSession_pong &>(obj));
      return true;
    case validatorSession_stats::ID:
      func(static_cast<validatorSession_stats &>(obj));
      return true;
    case validatorSession_statsProducer::ID:
      func(static_cast<validatorSession_statsProducer &>(obj));
      return true;
    case validatorSession_statsRound::ID:
      func(static_cast<validatorSession_statsRound &>(obj));
      return true;
    case validatorSession_round_id::ID:
      func(static_cast<validatorSession_round_id &>(obj));
      return true;
    case validatorSession_message_submittedBlock::ID:
      func(static_cast<validatorSession_message_submittedBlock &>(obj));
      return true;
    case validatorSession_message_approvedBlock::ID:
      func(static_cast<validatorSession_message_approvedBlock &>(obj));
      return true;
    case validatorSession_message_rejectedBlock::ID:
      func(static_cast<validatorSession_message_rejectedBlock &>(obj));
      return true;
    case validatorSession_message_commit::ID:
      func(static_cast<validatorSession_message_commit &>(obj));
      return true;
    case validatorSession_message_vote::ID:
      func(static_cast<validatorSession_message_vote &>(obj));
      return true;
    case validatorSession_message_voteFor::ID:
      func(static_cast<validatorSession_message_voteFor &>(obj));
      return true;
    case validatorSession_message_precommit::ID:
      func(static_cast<validatorSession_message_precommit &>(obj));
      return true;
    case validatorSession_message_empty::ID:
      func(static_cast<validatorSession_message_empty &>(obj));
      return true;
    case validatorSession_candidate_id::ID:
      func(static_cast<validatorSession_candidate_id &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(Object &obj, const T &func) {
switch (obj.get_id()) {    case hashable_bool::ID:
      func(create_tl_object<hashable_bool>());
      return true;
    case hashable_int32::ID:
      func(create_tl_object<hashable_int32>());
      return true;
    case hashable_int64::ID:
      func(create_tl_object<hashable_int64>());
      return true;
    case hashable_int256::ID:
      func(create_tl_object<hashable_int256>());
      return true;
    case hashable_bytes::ID:
      func(create_tl_object<hashable_bytes>());
      return true;
    case hashable_pair::ID:
      func(create_tl_object<hashable_pair>());
      return true;
    case hashable_vector::ID:
      func(create_tl_object<hashable_vector>());
      return true;
    case hashable_validatorSessionOldRound::ID:
      func(create_tl_object<hashable_validatorSessionOldRound>());
      return true;
    case hashable_validatorSessionRoundAttempt::ID:
      func(create_tl_object<hashable_validatorSessionRoundAttempt>());
      return true;
    case hashable_validatorSessionRound::ID:
      func(create_tl_object<hashable_validatorSessionRound>());
      return true;
    case hashable_blockSignature::ID:
      func(create_tl_object<hashable_blockSignature>());
      return true;
    case hashable_sentBlock::ID:
      func(create_tl_object<hashable_sentBlock>());
      return true;
    case hashable_sentBlockEmpty::ID:
      func(create_tl_object<hashable_sentBlockEmpty>());
      return true;
    case hashable_vote::ID:
      func(create_tl_object<hashable_vote>());
      return true;
    case hashable_blockCandidate::ID:
      func(create_tl_object<hashable_blockCandidate>());
      return true;
    case hashable_blockVoteCandidate::ID:
      func(create_tl_object<hashable_blockVoteCandidate>());
      return true;
    case hashable_blockCandidateAttempt::ID:
      func(create_tl_object<hashable_blockCandidateAttempt>());
      return true;
    case hashable_cntVector::ID:
      func(create_tl_object<hashable_cntVector>());
      return true;
    case hashable_cntSortedVector::ID:
      func(create_tl_object<hashable_cntSortedVector>());
      return true;
    case hashable_validatorSession::ID:
      func(create_tl_object<hashable_validatorSession>());
      return true;
    case storage_ok::ID:
      func(create_tl_object<storage_ok>());
      return true;
    case pk_unenc::ID:
      func(create_tl_object<pk_unenc>());
      return true;
    case pk_ed25519::ID:
      func(create_tl_object<pk_ed25519>());
      return true;
    case pk_aes::ID:
      func(create_tl_object<pk_aes>());
      return true;
    case pk_overlay::ID:
      func(create_tl_object<pk_overlay>());
      return true;
    case pub_unenc::ID:
      func(create_tl_object<pub_unenc>());
      return true;
    case pub_ed25519::ID:
      func(create_tl_object<pub_ed25519>());
      return true;
    case pub_aes::ID:
      func(create_tl_object<pub_aes>());
      return true;
    case pub_overlay::ID:
      func(create_tl_object<pub_overlay>());
      return true;
    case testObject::ID:
      func(create_tl_object<testObject>());
      return true;
    case testString::ID:
      func(create_tl_object<testString>());
      return true;
    case testInt::ID:
      func(create_tl_object<testInt>());
      return true;
    case testVectorBytes::ID:
      func(create_tl_object<testVectorBytes>());
      return true;
    case adnl_address_udp::ID:
      func(create_tl_object<adnl_address_udp>());
      return true;
    case adnl_address_udp6::ID:
      func(create_tl_object<adnl_address_udp6>());
      return true;
    case adnl_address_tunnel::ID:
      func(create_tl_object<adnl_address_tunnel>());
      return true;
    case adnl_address_reverse::ID:
      func(create_tl_object<adnl_address_reverse>());
      return true;
    case adnl_addressList::ID:
      func(create_tl_object<adnl_addressList>());
      return true;
    case adnl_message_createChannel::ID:
      func(create_tl_object<adnl_message_createChannel>());
      return true;
    case adnl_message_confirmChannel::ID:
      func(create_tl_object<adnl_message_confirmChannel>());
      return true;
    case adnl_message_custom::ID:
      func(create_tl_object<adnl_message_custom>());
      return true;
    case adnl_message_nop::ID:
      func(create_tl_object<adnl_message_nop>());
      return true;
    case adnl_message_reinit::ID:
      func(create_tl_object<adnl_message_reinit>());
      return true;
    case adnl_message_query::ID:
      func(create_tl_object<adnl_message_query>());
      return true;
    case adnl_message_answer::ID:
      func(create_tl_object<adnl_message_answer>());
      return true;
    case adnl_message_part::ID:
      func(create_tl_object<adnl_message_part>());
      return true;
    case adnl_node::ID:
      func(create_tl_object<adnl_node>());
      return true;
    case adnl_nodes::ID:
      func(create_tl_object<adnl_nodes>());
      return true;
    case adnl_packetContents::ID:
      func(create_tl_object<adnl_packetContents>());
      return true;
    case adnl_pong::ID:
      func(create_tl_object<adnl_pong>());
      return true;
    case adnl_proxy_none::ID:
      func(create_tl_object<adnl_proxy_none>());
      return true;
    case adnl_proxy_fast::ID:
      func(create_tl_object<adnl_proxy_fast>());
      return true;
    case adnl_proxyControlPacketPing::ID:
      func(create_tl_object<adnl_proxyControlPacketPing>());
      return true;
    case adnl_proxyControlPacketPong::ID:
      func(create_tl_object<adnl_proxyControlPacketPong>());
      return true;
    case adnl_proxyControlPacketRegister::ID:
      func(create_tl_object<adnl_proxyControlPacketRegister>());
      return true;
    case adnl_proxyPacketHeader::ID:
      func(create_tl_object<adnl_proxyPacketHeader>());
      return true;
    case adnl_proxyToFastHash::ID:
      func(create_tl_object<adnl_proxyToFastHash>());
      return true;
    case adnl_proxyToFast::ID:
      func(create_tl_object<adnl_proxyToFast>());
      return true;
    case adnl_tunnelPacketContents::ID:
      func(create_tl_object<adnl_tunnelPacketContents>());
      return true;
    case adnl_config_global::ID:
      func(create_tl_object<adnl_config_global>());
      return true;
    case adnl_db_node_key::ID:
      func(create_tl_object<adnl_db_node_key>());
      return true;
    case adnl_db_node_value::ID:
      func(create_tl_object<adnl_db_node_value>());
      return true;
    case adnl_id_short::ID:
      func(create_tl_object<adnl_id_short>());
      return true;
    case catchain_block::ID:
      func(create_tl_object<catchain_block>());
      return true;
    case catchain_blockNotFound::ID:
      func(create_tl_object<catchain_blockNotFound>());
      return true;
    case catchain_blockResult::ID:
      func(create_tl_object<catchain_blockResult>());
      return true;
    case catchain_blocks::ID:
      func(create_tl_object<catchain_blocks>());
      return true;
    case catchain_difference::ID:
      func(create_tl_object<catchain_difference>());
      return true;
    case catchain_differenceFork::ID:
      func(create_tl_object<catchain_differenceFork>());
      return true;
    case catchain_firstblock::ID:
      func(create_tl_object<catchain_firstblock>());
      return true;
    case catchain_sent::ID:
      func(create_tl_object<catchain_sent>());
      return true;
    case catchain_blockUpdate::ID:
      func(create_tl_object<catchain_blockUpdate>());
      return true;
    case catchain_block_data::ID:
      func(create_tl_object<catchain_block_data>());
      return true;
    case catchain_block_dep::ID:
      func(create_tl_object<catchain_block_dep>());
      return true;
    case catchain_block_id::ID:
      func(create_tl_object<catchain_block_id>());
      return true;
    case catchain_block_data_badBlock::ID:
      func(create_tl_object<catchain_block_data_badBlock>());
      return true;
    case catchain_block_data_fork::ID:
      func(create_tl_object<catchain_block_data_fork>());
      return true;
    case catchain_block_data_nop::ID:
      func(create_tl_object<catchain_block_data_nop>());
      return true;
    case catchain_config_global::ID:
      func(create_tl_object<catchain_config_global>());
      return true;
    case config_global::ID:
      func(create_tl_object<config_global>());
      return true;
    case config_local::ID:
      func(create_tl_object<config_local>());
      return true;
    case control_config_local::ID:
      func(create_tl_object<control_config_local>());
      return true;
    case db_candidate::ID:
      func(create_tl_object<db_candidate>());
      return true;
    case db_block_info::ID:
      func(create_tl_object<db_block_info>());
      return true;
    case db_block_packedInfo::ID:
      func(create_tl_object<db_block_packedInfo>());
      return true;
    case db_block_archivedInfo::ID:
      func(create_tl_object<db_block_archivedInfo>());
      return true;
    case db_blockdb_key_lru::ID:
      func(create_tl_object<db_blockdb_key_lru>());
      return true;
    case db_blockdb_key_value::ID:
      func(create_tl_object<db_blockdb_key_value>());
      return true;
    case db_blockdb_lru::ID:
      func(create_tl_object<db_blockdb_lru>());
      return true;
    case db_blockdb_value::ID:
      func(create_tl_object<db_blockdb_value>());
      return true;
    case db_candidate_id::ID:
      func(create_tl_object<db_candidate_id>());
      return true;
    case db_celldb_value::ID:
      func(create_tl_object<db_celldb_value>());
      return true;
    case db_celldb_key_value::ID:
      func(create_tl_object<db_celldb_key_value>());
      return true;
    case db_filedb_key_empty::ID:
      func(create_tl_object<db_filedb_key_empty>());
      return true;
    case db_filedb_key_blockFile::ID:
      func(create_tl_object<db_filedb_key_blockFile>());
      return true;
    case db_filedb_key_zeroStateFile::ID:
      func(create_tl_object<db_filedb_key_zeroStateFile>());
      return true;
    case db_filedb_key_persistentStateFile::ID:
      func(create_tl_object<db_filedb_key_persistentStateFile>());
      return true;
    case db_filedb_key_proof::ID:
      func(create_tl_object<db_filedb_key_proof>());
      return true;
    case db_filedb_key_proofLink::ID:
      func(create_tl_object<db_filedb_key_proofLink>());
      return true;
    case db_filedb_key_signatures::ID:
      func(create_tl_object<db_filedb_key_signatures>());
      return true;
    case db_filedb_key_candidate::ID:
      func(create_tl_object<db_filedb_key_candidate>());
      return true;
    case db_filedb_key_blockInfo::ID:
      func(create_tl_object<db_filedb_key_blockInfo>());
      return true;
    case db_filedb_value::ID:
      func(create_tl_object<db_filedb_value>());
      return true;
    case db_files_index_key::ID:
      func(create_tl_object<db_files_index_key>());
      return true;
    case db_files_package_key::ID:
      func(create_tl_object<db_files_package_key>());
      return true;
    case db_files_index_value::ID:
      func(create_tl_object<db_files_index_value>());
      return true;
    case db_files_package_firstBlock::ID:
      func(create_tl_object<db_files_package_firstBlock>());
      return true;
    case db_files_package_value::ID:
      func(create_tl_object<db_files_package_value>());
      return true;
    case db_lt_el_key::ID:
      func(create_tl_object<db_lt_el_key>());
      return true;
    case db_lt_desc_key::ID:
      func(create_tl_object<db_lt_desc_key>());
      return true;
    case db_lt_shard_key::ID:
      func(create_tl_object<db_lt_shard_key>());
      return true;
    case db_lt_status_key::ID:
      func(create_tl_object<db_lt_status_key>());
      return true;
    case db_lt_desc_value::ID:
      func(create_tl_object<db_lt_desc_value>());
      return true;
    case db_lt_el_value::ID:
      func(create_tl_object<db_lt_el_value>());
      return true;
    case db_lt_shard_value::ID:
      func(create_tl_object<db_lt_shard_value>());
      return true;
    case db_lt_status_value::ID:
      func(create_tl_object<db_lt_status_value>());
      return true;
    case db_root_config::ID:
      func(create_tl_object<db_root_config>());
      return true;
    case db_root_dbDescription::ID:
      func(create_tl_object<db_root_dbDescription>());
      return true;
    case db_root_key_cellDb::ID:
      func(create_tl_object<db_root_key_cellDb>());
      return true;
    case db_root_key_blockDb::ID:
      func(create_tl_object<db_root_key_blockDb>());
      return true;
    case db_root_key_config::ID:
      func(create_tl_object<db_root_key_config>());
      return true;
    case db_state_asyncSerializer::ID:
      func(create_tl_object<db_state_asyncSerializer>());
      return true;
    case db_state_dbVersion::ID:
      func(create_tl_object<db_state_dbVersion>());
      return true;
    case db_state_destroyedSessions::ID:
      func(create_tl_object<db_state_destroyedSessions>());
      return true;
    case db_state_gcBlockId::ID:
      func(create_tl_object<db_state_gcBlockId>());
      return true;
    case db_state_hardforks::ID:
      func(create_tl_object<db_state_hardforks>());
      return true;
    case db_state_initBlockId::ID:
      func(create_tl_object<db_state_initBlockId>());
      return true;
    case db_state_key_destroyedSessions::ID:
      func(create_tl_object<db_state_key_destroyedSessions>());
      return true;
    case db_state_key_initBlockId::ID:
      func(create_tl_object<db_state_key_initBlockId>());
      return true;
    case db_state_key_gcBlockId::ID:
      func(create_tl_object<db_state_key_gcBlockId>());
      return true;
    case db_state_key_shardClient::ID:
      func(create_tl_object<db_state_key_shardClient>());
      return true;
    case db_state_key_asyncSerializer::ID:
      func(create_tl_object<db_state_key_asyncSerializer>());
      return true;
    case db_state_key_hardforks::ID:
      func(create_tl_object<db_state_key_hardforks>());
      return true;
    case db_state_key_dbVersion::ID:
      func(create_tl_object<db_state_key_dbVersion>());
      return true;
    case db_state_shardClient::ID:
      func(create_tl_object<db_state_shardClient>());
      return true;
    case dht_key::ID:
      func(create_tl_object<dht_key>());
      return true;
    case dht_keyDescription::ID:
      func(create_tl_object<dht_keyDescription>());
      return true;
    case dht_message::ID:
      func(create_tl_object<dht_message>());
      return true;
    case dht_node::ID:
      func(create_tl_object<dht_node>());
      return true;
    case dht_nodes::ID:
      func(create_tl_object<dht_nodes>());
      return true;
    case dht_pong::ID:
      func(create_tl_object<dht_pong>());
      return true;
    case dht_requestReversePingCont::ID:
      func(create_tl_object<dht_requestReversePingCont>());
      return true;
    case dht_clientNotFound::ID:
      func(create_tl_object<dht_clientNotFound>());
      return true;
    case dht_reversePingOk::ID:
      func(create_tl_object<dht_reversePingOk>());
      return true;
    case dht_stored::ID:
      func(create_tl_object<dht_stored>());
      return true;
    case dht_updateRule_signature::ID:
      func(create_tl_object<dht_updateRule_signature>());
      return true;
    case dht_updateRule_anybody::ID:
      func(create_tl_object<dht_updateRule_anybody>());
      return true;
    case dht_updateRule_overlayNodes::ID:
      func(create_tl_object<dht_updateRule_overlayNodes>());
      return true;
    case dht_value::ID:
      func(create_tl_object<dht_value>());
      return true;
    case dht_valueNotFound::ID:
      func(create_tl_object<dht_valueNotFound>());
      return true;
    case dht_valueFound::ID:
      func(create_tl_object<dht_valueFound>());
      return true;
    case dht_config_global::ID:
      func(create_tl_object<dht_config_global>());
      return true;
    case dht_config_global_v2::ID:
      func(create_tl_object<dht_config_global_v2>());
      return true;
    case dht_config_local::ID:
      func(create_tl_object<dht_config_local>());
      return true;
    case dht_config_random_local::ID:
      func(create_tl_object<dht_config_random_local>());
      return true;
    case dht_db_bucket::ID:
      func(create_tl_object<dht_db_bucket>());
      return true;
    case dht_db_key_bucket::ID:
      func(create_tl_object<dht_db_key_bucket>());
      return true;
    case dummyworkchain0_config_global::ID:
      func(create_tl_object<dummyworkchain0_config_global>());
      return true;
    case engine_addr::ID:
      func(create_tl_object<engine_addr>());
      return true;
    case engine_addrProxy::ID:
      func(create_tl_object<engine_addrProxy>());
      return true;
    case engine_adnl::ID:
      func(create_tl_object<engine_adnl>());
      return true;
    case engine_controlInterface::ID:
      func(create_tl_object<engine_controlInterface>());
      return true;
    case engine_controlProcess::ID:
      func(create_tl_object<engine_controlProcess>());
      return true;
    case engine_dht::ID:
      func(create_tl_object<engine_dht>());
      return true;
    case engine_gc::ID:
      func(create_tl_object<engine_gc>());
      return true;
    case engine_liteServer::ID:
      func(create_tl_object<engine_liteServer>());
      return true;
    case engine_validator::ID:
      func(create_tl_object<engine_validator>());
      return true;
    case engine_validatorAdnlAddress::ID:
      func(create_tl_object<engine_validatorAdnlAddress>());
      return true;
    case engine_validatorTempKey::ID:
      func(create_tl_object<engine_validatorTempKey>());
      return true;
    case engine_adnlProxy_config::ID:
      func(create_tl_object<engine_adnlProxy_config>());
      return true;
    case engine_adnlProxy_port::ID:
      func(create_tl_object<engine_adnlProxy_port>());
      return true;
    case engine_dht_config::ID:
      func(create_tl_object<engine_dht_config>());
      return true;
    case engine_validator_config::ID:
      func(create_tl_object<engine_validator_config>());
      return true;
    case engine_validator_controlQueryError::ID:
      func(create_tl_object<engine_validator_controlQueryError>());
      return true;
    case engine_validator_dhtServerStatus::ID:
      func(create_tl_object<engine_validator_dhtServerStatus>());
      return true;
    case engine_validator_dhtServersStatus::ID:
      func(create_tl_object<engine_validator_dhtServersStatus>());
      return true;
    case engine_validator_electionBid::ID:
      func(create_tl_object<engine_validator_electionBid>());
      return true;
    case engine_validator_fullNodeMaster::ID:
      func(create_tl_object<engine_validator_fullNodeMaster>());
      return true;
    case engine_validator_fullNodeSlave::ID:
      func(create_tl_object<engine_validator_fullNodeSlave>());
      return true;
    case validator_groupMember::ID:
      func(create_tl_object<validator_groupMember>());
      return true;
    case engine_validator_jsonConfig::ID:
      func(create_tl_object<engine_validator_jsonConfig>());
      return true;
    case engine_validator_keyHash::ID:
      func(create_tl_object<engine_validator_keyHash>());
      return true;
    case engine_validator_onePerfTimerStat::ID:
      func(create_tl_object<engine_validator_onePerfTimerStat>());
      return true;
    case engine_validator_oneStat::ID:
      func(create_tl_object<engine_validator_oneStat>());
      return true;
    case engine_validator_overlayStats::ID:
      func(create_tl_object<engine_validator_overlayStats>());
      return true;
    case engine_validator_overlayStatsNode::ID:
      func(create_tl_object<engine_validator_overlayStatsNode>());
      return true;
    case engine_validator_overlaysStats::ID:
      func(create_tl_object<engine_validator_overlaysStats>());
      return true;
    case engine_validator_perfTimerStats::ID:
      func(create_tl_object<engine_validator_perfTimerStats>());
      return true;
    case engine_validator_perfTimerStatsByName::ID:
      func(create_tl_object<engine_validator_perfTimerStatsByName>());
      return true;
    case engine_validator_proposalVote::ID:
      func(create_tl_object<engine_validator_proposalVote>());
      return true;
    case engine_validator_signature::ID:
      func(create_tl_object<engine_validator_signature>());
      return true;
    case engine_validator_stats::ID:
      func(create_tl_object<engine_validator_stats>());
      return true;
    case engine_validator_success::ID:
      func(create_tl_object<engine_validator_success>());
      return true;
    case engine_validator_time::ID:
      func(create_tl_object<engine_validator_time>());
      return true;
    case fec_raptorQ::ID:
      func(create_tl_object<fec_raptorQ>());
      return true;
    case fec_roundRobin::ID:
      func(create_tl_object<fec_roundRobin>());
      return true;
    case fec_online::ID:
      func(create_tl_object<fec_online>());
      return true;
    case http_header::ID:
      func(create_tl_object<http_header>());
      return true;
    case http_payloadPart::ID:
      func(create_tl_object<http_payloadPart>());
      return true;
    case http_response::ID:
      func(create_tl_object<http_response>());
      return true;
    case http_proxy_capabilities::ID:
      func(create_tl_object<http_proxy_capabilities>());
      return true;
    case http_server_config::ID:
      func(create_tl_object<http_server_config>());
      return true;
    case http_server_dnsEntry::ID:
      func(create_tl_object<http_server_dnsEntry>());
      return true;
    case http_server_host::ID:
      func(create_tl_object<http_server_host>());
      return true;
    case id_config_local::ID:
      func(create_tl_object<id_config_local>());
      return true;
    case liteclient_config_global::ID:
      func(create_tl_object<liteclient_config_global>());
      return true;
    case liteserver_desc::ID:
      func(create_tl_object<liteserver_desc>());
      return true;
    case liteserver_config_local::ID:
      func(create_tl_object<liteserver_config_local>());
      return true;
    case liteserver_config_random_local::ID:
      func(create_tl_object<liteserver_config_random_local>());
      return true;
    case overlay_fec_received::ID:
      func(create_tl_object<overlay_fec_received>());
      return true;
    case overlay_fec_completed::ID:
      func(create_tl_object<overlay_fec_completed>());
      return true;
    case overlay_unicast::ID:
      func(create_tl_object<overlay_unicast>());
      return true;
    case overlay_broadcast::ID:
      func(create_tl_object<overlay_broadcast>());
      return true;
    case overlay_broadcastFec::ID:
      func(create_tl_object<overlay_broadcastFec>());
      return true;
    case overlay_broadcastFecShort::ID:
      func(create_tl_object<overlay_broadcastFecShort>());
      return true;
    case overlay_broadcastNotFound::ID:
      func(create_tl_object<overlay_broadcastNotFound>());
      return true;
    case overlay_broadcastList::ID:
      func(create_tl_object<overlay_broadcastList>());
      return true;
    case overlay_certificate::ID:
      func(create_tl_object<overlay_certificate>());
      return true;
    case overlay_certificateV2::ID:
      func(create_tl_object<overlay_certificateV2>());
      return true;
    case overlay_emptyCertificate::ID:
      func(create_tl_object<overlay_emptyCertificate>());
      return true;
    case overlay_certificateId::ID:
      func(create_tl_object<overlay_certificateId>());
      return true;
    case overlay_certificateIdV2::ID:
      func(create_tl_object<overlay_certificateIdV2>());
      return true;
    case overlay_message::ID:
      func(create_tl_object<overlay_message>());
      return true;
    case overlay_node::ID:
      func(create_tl_object<overlay_node>());
      return true;
    case overlay_nodes::ID:
      func(create_tl_object<overlay_nodes>());
      return true;
    case overlay_broadcast_id::ID:
      func(create_tl_object<overlay_broadcast_id>());
      return true;
    case overlay_broadcast_toSign::ID:
      func(create_tl_object<overlay_broadcast_toSign>());
      return true;
    case overlay_broadcastFec_id::ID:
      func(create_tl_object<overlay_broadcastFec_id>());
      return true;
    case overlay_broadcastFec_partId::ID:
      func(create_tl_object<overlay_broadcastFec_partId>());
      return true;
    case overlay_db_key_nodes::ID:
      func(create_tl_object<overlay_db_key_nodes>());
      return true;
    case overlay_db_nodes::ID:
      func(create_tl_object<overlay_db_nodes>());
      return true;
    case overlay_node_toSign::ID:
      func(create_tl_object<overlay_node_toSign>());
      return true;
    case rldp_message::ID:
      func(create_tl_object<rldp_message>());
      return true;
    case rldp_query::ID:
      func(create_tl_object<rldp_query>());
      return true;
    case rldp_answer::ID:
      func(create_tl_object<rldp_answer>());
      return true;
    case rldp_messagePart::ID:
      func(create_tl_object<rldp_messagePart>());
      return true;
    case rldp_confirm::ID:
      func(create_tl_object<rldp_confirm>());
      return true;
    case rldp_complete::ID:
      func(create_tl_object<rldp_complete>());
      return true;
    case rldp2_messagePart::ID:
      func(create_tl_object<rldp2_messagePart>());
      return true;
    case rldp2_confirm::ID:
      func(create_tl_object<rldp2_confirm>());
      return true;
    case rldp2_complete::ID:
      func(create_tl_object<rldp2_complete>());
      return true;
    case storage_piece::ID:
      func(create_tl_object<storage_piece>());
      return true;
    case storage_pong::ID:
      func(create_tl_object<storage_pong>());
      return true;
    case storage_priorityAction_all::ID:
      func(create_tl_object<storage_priorityAction_all>());
      return true;
    case storage_priorityAction_idx::ID:
      func(create_tl_object<storage_priorityAction_idx>());
      return true;
    case storage_priorityAction_name::ID:
      func(create_tl_object<storage_priorityAction_name>());
      return true;
    case storage_state::ID:
      func(create_tl_object<storage_state>());
      return true;
    case storage_torrentInfo::ID:
      func(create_tl_object<storage_torrentInfo>());
      return true;
    case storage_updateInit::ID:
      func(create_tl_object<storage_updateInit>());
      return true;
    case storage_updateHavePieces::ID:
      func(create_tl_object<storage_updateHavePieces>());
      return true;
    case storage_updateState::ID:
      func(create_tl_object<storage_updateState>());
      return true;
    case storage_daemon_contractInfo::ID:
      func(create_tl_object<storage_daemon_contractInfo>());
      return true;
    case storage_daemon_fileInfo::ID:
      func(create_tl_object<storage_daemon_fileInfo>());
      return true;
    case storage_daemon_keyHash::ID:
      func(create_tl_object<storage_daemon_keyHash>());
      return true;
    case storage_daemon_newContractMessage::ID:
      func(create_tl_object<storage_daemon_newContractMessage>());
      return true;
    case storage_daemon_newContractParams::ID:
      func(create_tl_object<storage_daemon_newContractParams>());
      return true;
    case storage_daemon_newContractParamsAuto::ID:
      func(create_tl_object<storage_daemon_newContractParamsAuto>());
      return true;
    case storage_daemon_peer::ID:
      func(create_tl_object<storage_daemon_peer>());
      return true;
    case storage_daemon_peerList::ID:
      func(create_tl_object<storage_daemon_peerList>());
      return true;
    case storage_daemon_providerAddress::ID:
      func(create_tl_object<storage_daemon_providerAddress>());
      return true;
    case storage_daemon_providerConfig::ID:
      func(create_tl_object<storage_daemon_providerConfig>());
      return true;
    case storage_daemon_providerInfo::ID:
      func(create_tl_object<storage_daemon_providerInfo>());
      return true;
    case storage_daemon_queryError::ID:
      func(create_tl_object<storage_daemon_queryError>());
      return true;
    case storage_daemon_prioritySet::ID:
      func(create_tl_object<storage_daemon_prioritySet>());
      return true;
    case storage_daemon_priorityPending::ID:
      func(create_tl_object<storage_daemon_priorityPending>());
      return true;
    case storage_daemon_success::ID:
      func(create_tl_object<storage_daemon_success>());
      return true;
    case storage_daemon_torrent::ID:
      func(create_tl_object<storage_daemon_torrent>());
      return true;
    case storage_daemon_torrentFull::ID:
      func(create_tl_object<storage_daemon_torrentFull>());
      return true;
    case storage_daemon_torrentList::ID:
      func(create_tl_object<storage_daemon_torrentList>());
      return true;
    case storage_daemon_torrentMeta::ID:
      func(create_tl_object<storage_daemon_torrentMeta>());
      return true;
    case storage_daemon_config::ID:
      func(create_tl_object<storage_daemon_config>());
      return true;
    case storage_daemon_provider_params::ID:
      func(create_tl_object<storage_daemon_provider_params>());
      return true;
    case storage_provider_db_contractAddress::ID:
      func(create_tl_object<storage_provider_db_contractAddress>());
      return true;
    case storage_provider_db_contractList::ID:
      func(create_tl_object<storage_provider_db_contractList>());
      return true;
    case storage_db_piecesInDb::ID:
      func(create_tl_object<storage_db_piecesInDb>());
      return true;
    case storage_db_priorities::ID:
      func(create_tl_object<storage_db_priorities>());
      return true;
    case storage_db_torrentList::ID:
      func(create_tl_object<storage_db_torrentList>());
      return true;
    case storage_db_torrent::ID:
      func(create_tl_object<storage_db_torrent>());
      return true;
    case storage_db_key_pieceInDb::ID:
      func(create_tl_object<storage_db_key_pieceInDb>());
      return true;
    case storage_db_key_piecesInDb::ID:
      func(create_tl_object<storage_db_key_piecesInDb>());
      return true;
    case storage_db_key_priorities::ID:
      func(create_tl_object<storage_db_key_priorities>());
      return true;
    case storage_db_key_torrentList::ID:
      func(create_tl_object<storage_db_key_torrentList>());
      return true;
    case storage_db_key_torrentMeta::ID:
      func(create_tl_object<storage_db_key_torrentMeta>());
      return true;
    case storage_db_key_torrent::ID:
      func(create_tl_object<storage_db_key_torrent>());
      return true;
    case storage_provider_db_microchunkTree::ID:
      func(create_tl_object<storage_provider_db_microchunkTree>());
      return true;
    case storage_provider_db_state::ID:
      func(create_tl_object<storage_provider_db_state>());
      return true;
    case storage_provider_db_storageContract::ID:
      func(create_tl_object<storage_provider_db_storageContract>());
      return true;
    case storage_provider_db_key_contractList::ID:
      func(create_tl_object<storage_provider_db_key_contractList>());
      return true;
    case storage_provider_db_key_microchunkTree::ID:
      func(create_tl_object<storage_provider_db_key_microchunkTree>());
      return true;
    case storage_provider_db_key_providerConfig::ID:
      func(create_tl_object<storage_provider_db_key_providerConfig>());
      return true;
    case storage_provider_db_key_state::ID:
      func(create_tl_object<storage_provider_db_key_state>());
      return true;
    case storage_provider_db_key_storageContract::ID:
      func(create_tl_object<storage_provider_db_key_storageContract>());
      return true;
    case tcp_authentificate::ID:
      func(create_tl_object<tcp_authentificate>());
      return true;
    case tcp_authentificationNonce::ID:
      func(create_tl_object<tcp_authentificationNonce>());
      return true;
    case tcp_authentificationComplete::ID:
      func(create_tl_object<tcp_authentificationComplete>());
      return true;
    case tcp_pong::ID:
      func(create_tl_object<tcp_pong>());
      return true;
    case ton_blockId::ID:
      func(create_tl_object<ton_blockId>());
      return true;
    case ton_blockIdApprove::ID:
      func(create_tl_object<ton_blockIdApprove>());
      return true;
    case tonNode_archiveNotFound::ID:
      func(create_tl_object<tonNode_archiveNotFound>());
      return true;
    case tonNode_archiveInfo::ID:
      func(create_tl_object<tonNode_archiveInfo>());
      return true;
    case tonNode_blockDescriptionEmpty::ID:
      func(create_tl_object<tonNode_blockDescriptionEmpty>());
      return true;
    case tonNode_blockDescription::ID:
      func(create_tl_object<tonNode_blockDescription>());
      return true;
    case tonNode_blockId::ID:
      func(create_tl_object<tonNode_blockId>());
      return true;
    case tonNode_blockIdExt::ID:
      func(create_tl_object<tonNode_blockIdExt>());
      return true;
    case tonNode_blockSignature::ID:
      func(create_tl_object<tonNode_blockSignature>());
      return true;
    case tonNode_blocksDescription::ID:
      func(create_tl_object<tonNode_blocksDescription>());
      return true;
    case tonNode_blockBroadcast::ID:
      func(create_tl_object<tonNode_blockBroadcast>());
      return true;
    case tonNode_ihrMessageBroadcast::ID:
      func(create_tl_object<tonNode_ihrMessageBroadcast>());
      return true;
    case tonNode_externalMessageBroadcast::ID:
      func(create_tl_object<tonNode_externalMessageBroadcast>());
      return true;
    case tonNode_newShardBlockBroadcast::ID:
      func(create_tl_object<tonNode_newShardBlockBroadcast>());
      return true;
    case tonNode_capabilities::ID:
      func(create_tl_object<tonNode_capabilities>());
      return true;
    case tonNode_data::ID:
      func(create_tl_object<tonNode_data>());
      return true;
    case tonNode_dataFull::ID:
      func(create_tl_object<tonNode_dataFull>());
      return true;
    case tonNode_dataFullEmpty::ID:
      func(create_tl_object<tonNode_dataFullEmpty>());
      return true;
    case tonNode_dataList::ID:
      func(create_tl_object<tonNode_dataList>());
      return true;
    case tonNode_externalMessage::ID:
      func(create_tl_object<tonNode_externalMessage>());
      return true;
    case tonNode_ihrMessage::ID:
      func(create_tl_object<tonNode_ihrMessage>());
      return true;
    case tonNode_keyBlocks::ID:
      func(create_tl_object<tonNode_keyBlocks>());
      return true;
    case tonNode_newShardBlock::ID:
      func(create_tl_object<tonNode_newShardBlock>());
      return true;
    case tonNode_prepared::ID:
      func(create_tl_object<tonNode_prepared>());
      return true;
    case tonNode_notFound::ID:
      func(create_tl_object<tonNode_notFound>());
      return true;
    case tonNode_preparedProofEmpty::ID:
      func(create_tl_object<tonNode_preparedProofEmpty>());
      return true;
    case tonNode_preparedProof::ID:
      func(create_tl_object<tonNode_preparedProof>());
      return true;
    case tonNode_preparedProofLink::ID:
      func(create_tl_object<tonNode_preparedProofLink>());
      return true;
    case tonNode_preparedState::ID:
      func(create_tl_object<tonNode_preparedState>());
      return true;
    case tonNode_notFoundState::ID:
      func(create_tl_object<tonNode_notFoundState>());
      return true;
    case tonNode_sessionId::ID:
      func(create_tl_object<tonNode_sessionId>());
      return true;
    case tonNode_shardPublicOverlayId::ID:
      func(create_tl_object<tonNode_shardPublicOverlayId>());
      return true;
    case tonNode_success::ID:
      func(create_tl_object<tonNode_success>());
      return true;
    case tonNode_zeroStateIdExt::ID:
      func(create_tl_object<tonNode_zeroStateIdExt>());
      return true;
    case validator_group::ID:
      func(create_tl_object<validator_group>());
      return true;
    case validator_groupEx::ID:
      func(create_tl_object<validator_groupEx>());
      return true;
    case validator_groupNew::ID:
      func(create_tl_object<validator_groupNew>());
      return true;
    case validator_config_global::ID:
      func(create_tl_object<validator_config_global>());
      return true;
    case validator_config_local::ID:
      func(create_tl_object<validator_config_local>());
      return true;
    case validator_config_random_local::ID:
      func(create_tl_object<validator_config_random_local>());
      return true;
    case validatorSession_blockUpdate::ID:
      func(create_tl_object<validatorSession_blockUpdate>());
      return true;
    case validatorSession_candidate::ID:
      func(create_tl_object<validatorSession_candidate>());
      return true;
    case validatorSession_candidateId::ID:
      func(create_tl_object<validatorSession_candidateId>());
      return true;
    case validatorSession_catchainOptions::ID:
      func(create_tl_object<validatorSession_catchainOptions>());
      return true;
    case validatorSession_config::ID:
      func(create_tl_object<validatorSession_config>());
      return true;
    case validatorSession_configNew::ID:
      func(create_tl_object<validatorSession_configNew>());
      return true;
    case validatorSession_configVersioned::ID:
      func(create_tl_object<validatorSession_configVersioned>());
      return true;
    case validatorSession_configVersionedV2::ID:
      func(create_tl_object<validatorSession_configVersionedV2>());
      return true;
    case validatorSession_message_startSession::ID:
      func(create_tl_object<validatorSession_message_startSession>());
      return true;
    case validatorSession_message_finishSession::ID:
      func(create_tl_object<validatorSession_message_finishSession>());
      return true;
    case validatorSession_pong::ID:
      func(create_tl_object<validatorSession_pong>());
      return true;
    case validatorSession_stats::ID:
      func(create_tl_object<validatorSession_stats>());
      return true;
    case validatorSession_statsProducer::ID:
      func(create_tl_object<validatorSession_statsProducer>());
      return true;
    case validatorSession_statsRound::ID:
      func(create_tl_object<validatorSession_statsRound>());
      return true;
    case validatorSession_round_id::ID:
      func(create_tl_object<validatorSession_round_id>());
      return true;
    case validatorSession_message_submittedBlock::ID:
      func(create_tl_object<validatorSession_message_submittedBlock>());
      return true;
    case validatorSession_message_approvedBlock::ID:
      func(create_tl_object<validatorSession_message_approvedBlock>());
      return true;
    case validatorSession_message_rejectedBlock::ID:
      func(create_tl_object<validatorSession_message_rejectedBlock>());
      return true;
    case validatorSession_message_commit::ID:
      func(create_tl_object<validatorSession_message_commit>());
      return true;
    case validatorSession_message_vote::ID:
      func(create_tl_object<validatorSession_message_vote>());
      return true;
    case validatorSession_message_voteFor::ID:
      func(create_tl_object<validatorSession_message_voteFor>());
      return true;
    case validatorSession_message_precommit::ID:
      func(create_tl_object<validatorSession_message_precommit>());
      return true;
    case validatorSession_message_empty::ID:
      func(create_tl_object<validatorSession_message_empty>());
      return true;
    case validatorSession_candidate_id::ID:
      func(create_tl_object<validatorSession_candidate_id>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(Function &obj, const T &func) {
  switch (obj.get_id()) {
    case adnl_ping::ID:
      func(static_cast<adnl_ping &>(obj));
      return true;
    case catchain_getBlock::ID:
      func(static_cast<catchain_getBlock &>(obj));
      return true;
    case catchain_getBlockHistory::ID:
      func(static_cast<catchain_getBlockHistory &>(obj));
      return true;
    case catchain_getBlocks::ID:
      func(static_cast<catchain_getBlocks &>(obj));
      return true;
    case catchain_getDifference::ID:
      func(static_cast<catchain_getDifference &>(obj));
      return true;
    case dht_findNode::ID:
      func(static_cast<dht_findNode &>(obj));
      return true;
    case dht_findValue::ID:
      func(static_cast<dht_findValue &>(obj));
      return true;
    case dht_getSignedAddressList::ID:
      func(static_cast<dht_getSignedAddressList &>(obj));
      return true;
    case dht_ping::ID:
      func(static_cast<dht_ping &>(obj));
      return true;
    case dht_query::ID:
      func(static_cast<dht_query &>(obj));
      return true;
    case dht_registerReverseConnection::ID:
      func(static_cast<dht_registerReverseConnection &>(obj));
      return true;
    case dht_requestReversePing::ID:
      func(static_cast<dht_requestReversePing &>(obj));
      return true;
    case dht_store::ID:
      func(static_cast<dht_store &>(obj));
      return true;
    case engine_validator_addAdnlId::ID:
      func(static_cast<engine_validator_addAdnlId &>(obj));
      return true;
    case engine_validator_addControlInterface::ID:
      func(static_cast<engine_validator_addControlInterface &>(obj));
      return true;
    case engine_validator_addControlProcess::ID:
      func(static_cast<engine_validator_addControlProcess &>(obj));
      return true;
    case engine_validator_addDhtId::ID:
      func(static_cast<engine_validator_addDhtId &>(obj));
      return true;
    case engine_validator_addListeningPort::ID:
      func(static_cast<engine_validator_addListeningPort &>(obj));
      return true;
    case engine_validator_addLiteserver::ID:
      func(static_cast<engine_validator_addLiteserver &>(obj));
      return true;
    case engine_validator_addProxy::ID:
      func(static_cast<engine_validator_addProxy &>(obj));
      return true;
    case engine_validator_addValidatorAdnlAddress::ID:
      func(static_cast<engine_validator_addValidatorAdnlAddress &>(obj));
      return true;
    case engine_validator_addValidatorPermanentKey::ID:
      func(static_cast<engine_validator_addValidatorPermanentKey &>(obj));
      return true;
    case engine_validator_addValidatorTempKey::ID:
      func(static_cast<engine_validator_addValidatorTempKey &>(obj));
      return true;
    case engine_validator_changeFullNodeAdnlAddress::ID:
      func(static_cast<engine_validator_changeFullNodeAdnlAddress &>(obj));
      return true;
    case engine_validator_checkDhtServers::ID:
      func(static_cast<engine_validator_checkDhtServers &>(obj));
      return true;
    case engine_validator_controlQuery::ID:
      func(static_cast<engine_validator_controlQuery &>(obj));
      return true;
    case engine_validator_createComplaintVote::ID:
      func(static_cast<engine_validator_createComplaintVote &>(obj));
      return true;
    case engine_validator_createElectionBid::ID:
      func(static_cast<engine_validator_createElectionBid &>(obj));
      return true;
    case engine_validator_createProposalVote::ID:
      func(static_cast<engine_validator_createProposalVote &>(obj));
      return true;
    case engine_validator_delAdnlId::ID:
      func(static_cast<engine_validator_delAdnlId &>(obj));
      return true;
    case engine_validator_delDhtId::ID:
      func(static_cast<engine_validator_delDhtId &>(obj));
      return true;
    case engine_validator_delListeningPort::ID:
      func(static_cast<engine_validator_delListeningPort &>(obj));
      return true;
    case engine_validator_delProxy::ID:
      func(static_cast<engine_validator_delProxy &>(obj));
      return true;
    case engine_validator_delValidatorAdnlAddress::ID:
      func(static_cast<engine_validator_delValidatorAdnlAddress &>(obj));
      return true;
    case engine_validator_delValidatorPermanentKey::ID:
      func(static_cast<engine_validator_delValidatorPermanentKey &>(obj));
      return true;
    case engine_validator_delValidatorTempKey::ID:
      func(static_cast<engine_validator_delValidatorTempKey &>(obj));
      return true;
    case engine_validator_exportPrivateKey::ID:
      func(static_cast<engine_validator_exportPrivateKey &>(obj));
      return true;
    case engine_validator_exportPublicKey::ID:
      func(static_cast<engine_validator_exportPublicKey &>(obj));
      return true;
    case engine_validator_generateKeyPair::ID:
      func(static_cast<engine_validator_generateKeyPair &>(obj));
      return true;
    case engine_validator_getConfig::ID:
      func(static_cast<engine_validator_getConfig &>(obj));
      return true;
    case engine_validator_getOverlaysStats::ID:
      func(static_cast<engine_validator_getOverlaysStats &>(obj));
      return true;
    case engine_validator_getPerfTimerStats::ID:
      func(static_cast<engine_validator_getPerfTimerStats &>(obj));
      return true;
    case engine_validator_getStats::ID:
      func(static_cast<engine_validator_getStats &>(obj));
      return true;
    case engine_validator_getTime::ID:
      func(static_cast<engine_validator_getTime &>(obj));
      return true;
    case engine_validator_importCertificate::ID:
      func(static_cast<engine_validator_importCertificate &>(obj));
      return true;
    case engine_validator_importPrivateKey::ID:
      func(static_cast<engine_validator_importPrivateKey &>(obj));
      return true;
    case engine_validator_importShardOverlayCertificate::ID:
      func(static_cast<engine_validator_importShardOverlayCertificate &>(obj));
      return true;
    case engine_validator_setVerbosity::ID:
      func(static_cast<engine_validator_setVerbosity &>(obj));
      return true;
    case engine_validator_sign::ID:
      func(static_cast<engine_validator_sign &>(obj));
      return true;
    case engine_validator_signShardOverlayCertificate::ID:
      func(static_cast<engine_validator_signShardOverlayCertificate &>(obj));
      return true;
    case getTestObject::ID:
      func(static_cast<getTestObject &>(obj));
      return true;
    case http_getNextPayloadPart::ID:
      func(static_cast<http_getNextPayloadPart &>(obj));
      return true;
    case http_proxy_getCapabilities::ID:
      func(static_cast<http_proxy_getCapabilities &>(obj));
      return true;
    case http_request::ID:
      func(static_cast<http_request &>(obj));
      return true;
    case overlay_getBroadcast::ID:
      func(static_cast<overlay_getBroadcast &>(obj));
      return true;
    case overlay_getBroadcastList::ID:
      func(static_cast<overlay_getBroadcastList &>(obj));
      return true;
    case overlay_getRandomPeers::ID:
      func(static_cast<overlay_getRandomPeers &>(obj));
      return true;
    case overlay_query::ID:
      func(static_cast<overlay_query &>(obj));
      return true;
    case storage_addUpdate::ID:
      func(static_cast<storage_addUpdate &>(obj));
      return true;
    case storage_daemon_addByHash::ID:
      func(static_cast<storage_daemon_addByHash &>(obj));
      return true;
    case storage_daemon_addByMeta::ID:
      func(static_cast<storage_daemon_addByMeta &>(obj));
      return true;
    case storage_daemon_closeStorageContract::ID:
      func(static_cast<storage_daemon_closeStorageContract &>(obj));
      return true;
    case storage_daemon_createTorrent::ID:
      func(static_cast<storage_daemon_createTorrent &>(obj));
      return true;
    case storage_daemon_deployProvider::ID:
      func(static_cast<storage_daemon_deployProvider &>(obj));
      return true;
    case storage_daemon_getNewContractMessage::ID:
      func(static_cast<storage_daemon_getNewContractMessage &>(obj));
      return true;
    case storage_daemon_getProviderInfo::ID:
      func(static_cast<storage_daemon_getProviderInfo &>(obj));
      return true;
    case storage_daemon_getProviderParams::ID:
      func(static_cast<storage_daemon_getProviderParams &>(obj));
      return true;
    case storage_daemon_getTorrentFull::ID:
      func(static_cast<storage_daemon_getTorrentFull &>(obj));
      return true;
    case storage_daemon_getTorrentMeta::ID:
      func(static_cast<storage_daemon_getTorrentMeta &>(obj));
      return true;
    case storage_daemon_getTorrentPeers::ID:
      func(static_cast<storage_daemon_getTorrentPeers &>(obj));
      return true;
    case storage_daemon_getTorrents::ID:
      func(static_cast<storage_daemon_getTorrents &>(obj));
      return true;
    case storage_daemon_importPrivateKey::ID:
      func(static_cast<storage_daemon_importPrivateKey &>(obj));
      return true;
    case storage_daemon_initProvider::ID:
      func(static_cast<storage_daemon_initProvider &>(obj));
      return true;
    case storage_daemon_loadFrom::ID:
      func(static_cast<storage_daemon_loadFrom &>(obj));
      return true;
    case storage_daemon_removeStorageProvider::ID:
      func(static_cast<storage_daemon_removeStorageProvider &>(obj));
      return true;
    case storage_daemon_removeTorrent::ID:
      func(static_cast<storage_daemon_removeTorrent &>(obj));
      return true;
    case storage_daemon_sendCoins::ID:
      func(static_cast<storage_daemon_sendCoins &>(obj));
      return true;
    case storage_daemon_setActiveDownload::ID:
      func(static_cast<storage_daemon_setActiveDownload &>(obj));
      return true;
    case storage_daemon_setActiveUpload::ID:
      func(static_cast<storage_daemon_setActiveUpload &>(obj));
      return true;
    case storage_daemon_setFilePriorityAll::ID:
      func(static_cast<storage_daemon_setFilePriorityAll &>(obj));
      return true;
    case storage_daemon_setFilePriorityByIdx::ID:
      func(static_cast<storage_daemon_setFilePriorityByIdx &>(obj));
      return true;
    case storage_daemon_setFilePriorityByName::ID:
      func(static_cast<storage_daemon_setFilePriorityByName &>(obj));
      return true;
    case storage_daemon_setProviderConfig::ID:
      func(static_cast<storage_daemon_setProviderConfig &>(obj));
      return true;
    case storage_daemon_setProviderParams::ID:
      func(static_cast<storage_daemon_setProviderParams &>(obj));
      return true;
    case storage_daemon_setVerbosity::ID:
      func(static_cast<storage_daemon_setVerbosity &>(obj));
      return true;
    case storage_daemon_withdraw::ID:
      func(static_cast<storage_daemon_withdraw &>(obj));
      return true;
    case storage_getPiece::ID:
      func(static_cast<storage_getPiece &>(obj));
      return true;
    case storage_getTorrentInfo::ID:
      func(static_cast<storage_getTorrentInfo &>(obj));
      return true;
    case storage_ping::ID:
      func(static_cast<storage_ping &>(obj));
      return true;
    case tcp_ping::ID:
      func(static_cast<tcp_ping &>(obj));
      return true;
    case tonNode_downloadBlock::ID:
      func(static_cast<tonNode_downloadBlock &>(obj));
      return true;
    case tonNode_downloadBlockFull::ID:
      func(static_cast<tonNode_downloadBlockFull &>(obj));
      return true;
    case tonNode_downloadBlockProof::ID:
      func(static_cast<tonNode_downloadBlockProof &>(obj));
      return true;
    case tonNode_downloadBlockProofLink::ID:
      func(static_cast<tonNode_downloadBlockProofLink &>(obj));
      return true;
    case tonNode_downloadBlockProofLinks::ID:
      func(static_cast<tonNode_downloadBlockProofLinks &>(obj));
      return true;
    case tonNode_downloadBlockProofs::ID:
      func(static_cast<tonNode_downloadBlockProofs &>(obj));
      return true;
    case tonNode_downloadBlocks::ID:
      func(static_cast<tonNode_downloadBlocks &>(obj));
      return true;
    case tonNode_downloadKeyBlockProof::ID:
      func(static_cast<tonNode_downloadKeyBlockProof &>(obj));
      return true;
    case tonNode_downloadKeyBlockProofLink::ID:
      func(static_cast<tonNode_downloadKeyBlockProofLink &>(obj));
      return true;
    case tonNode_downloadKeyBlockProofLinks::ID:
      func(static_cast<tonNode_downloadKeyBlockProofLinks &>(obj));
      return true;
    case tonNode_downloadKeyBlockProofs::ID:
      func(static_cast<tonNode_downloadKeyBlockProofs &>(obj));
      return true;
    case tonNode_downloadNextBlockFull::ID:
      func(static_cast<tonNode_downloadNextBlockFull &>(obj));
      return true;
    case tonNode_downloadPersistentState::ID:
      func(static_cast<tonNode_downloadPersistentState &>(obj));
      return true;
    case tonNode_downloadPersistentStateSlice::ID:
      func(static_cast<tonNode_downloadPersistentStateSlice &>(obj));
      return true;
    case tonNode_downloadZeroState::ID:
      func(static_cast<tonNode_downloadZeroState &>(obj));
      return true;
    case tonNode_getArchiveInfo::ID:
      func(static_cast<tonNode_getArchiveInfo &>(obj));
      return true;
    case tonNode_getArchiveSlice::ID:
      func(static_cast<tonNode_getArchiveSlice &>(obj));
      return true;
    case tonNode_getCapabilities::ID:
      func(static_cast<tonNode_getCapabilities &>(obj));
      return true;
    case tonNode_getNextBlockDescription::ID:
      func(static_cast<tonNode_getNextBlockDescription &>(obj));
      return true;
    case tonNode_getNextBlocksDescription::ID:
      func(static_cast<tonNode_getNextBlocksDescription &>(obj));
      return true;
    case tonNode_getNextKeyBlockIds::ID:
      func(static_cast<tonNode_getNextKeyBlockIds &>(obj));
      return true;
    case tonNode_getPrevBlocksDescription::ID:
      func(static_cast<tonNode_getPrevBlocksDescription &>(obj));
      return true;
    case tonNode_prepareBlock::ID:
      func(static_cast<tonNode_prepareBlock &>(obj));
      return true;
    case tonNode_prepareBlockProof::ID:
      func(static_cast<tonNode_prepareBlockProof &>(obj));
      return true;
    case tonNode_prepareBlockProofs::ID:
      func(static_cast<tonNode_prepareBlockProofs &>(obj));
      return true;
    case tonNode_prepareBlocks::ID:
      func(static_cast<tonNode_prepareBlocks &>(obj));
      return true;
    case tonNode_prepareKeyBlockProof::ID:
      func(static_cast<tonNode_prepareKeyBlockProof &>(obj));
      return true;
    case tonNode_prepareKeyBlockProofs::ID:
      func(static_cast<tonNode_prepareKeyBlockProofs &>(obj));
      return true;
    case tonNode_preparePersistentState::ID:
      func(static_cast<tonNode_preparePersistentState &>(obj));
      return true;
    case tonNode_prepareZeroState::ID:
      func(static_cast<tonNode_prepareZeroState &>(obj));
      return true;
    case tonNode_query::ID:
      func(static_cast<tonNode_query &>(obj));
      return true;
    case tonNode_slave_sendExtMessage::ID:
      func(static_cast<tonNode_slave_sendExtMessage &>(obj));
      return true;
    case validatorSession_downloadCandidate::ID:
      func(static_cast<validatorSession_downloadCandidate &>(obj));
      return true;
    case validatorSession_ping::ID:
      func(static_cast<validatorSession_ping &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(Function &obj, const T &func) {
switch (obj.get_id()) {    case adnl_ping::ID:
      func(create_tl_object<adnl_ping>());
      return true;
    case catchain_getBlock::ID:
      func(create_tl_object<catchain_getBlock>());
      return true;
    case catchain_getBlockHistory::ID:
      func(create_tl_object<catchain_getBlockHistory>());
      return true;
    case catchain_getBlocks::ID:
      func(create_tl_object<catchain_getBlocks>());
      return true;
    case catchain_getDifference::ID:
      func(create_tl_object<catchain_getDifference>());
      return true;
    case dht_findNode::ID:
      func(create_tl_object<dht_findNode>());
      return true;
    case dht_findValue::ID:
      func(create_tl_object<dht_findValue>());
      return true;
    case dht_getSignedAddressList::ID:
      func(create_tl_object<dht_getSignedAddressList>());
      return true;
    case dht_ping::ID:
      func(create_tl_object<dht_ping>());
      return true;
    case dht_query::ID:
      func(create_tl_object<dht_query>());
      return true;
    case dht_registerReverseConnection::ID:
      func(create_tl_object<dht_registerReverseConnection>());
      return true;
    case dht_requestReversePing::ID:
      func(create_tl_object<dht_requestReversePing>());
      return true;
    case dht_store::ID:
      func(create_tl_object<dht_store>());
      return true;
    case engine_validator_addAdnlId::ID:
      func(create_tl_object<engine_validator_addAdnlId>());
      return true;
    case engine_validator_addControlInterface::ID:
      func(create_tl_object<engine_validator_addControlInterface>());
      return true;
    case engine_validator_addControlProcess::ID:
      func(create_tl_object<engine_validator_addControlProcess>());
      return true;
    case engine_validator_addDhtId::ID:
      func(create_tl_object<engine_validator_addDhtId>());
      return true;
    case engine_validator_addListeningPort::ID:
      func(create_tl_object<engine_validator_addListeningPort>());
      return true;
    case engine_validator_addLiteserver::ID:
      func(create_tl_object<engine_validator_addLiteserver>());
      return true;
    case engine_validator_addProxy::ID:
      func(create_tl_object<engine_validator_addProxy>());
      return true;
    case engine_validator_addValidatorAdnlAddress::ID:
      func(create_tl_object<engine_validator_addValidatorAdnlAddress>());
      return true;
    case engine_validator_addValidatorPermanentKey::ID:
      func(create_tl_object<engine_validator_addValidatorPermanentKey>());
      return true;
    case engine_validator_addValidatorTempKey::ID:
      func(create_tl_object<engine_validator_addValidatorTempKey>());
      return true;
    case engine_validator_changeFullNodeAdnlAddress::ID:
      func(create_tl_object<engine_validator_changeFullNodeAdnlAddress>());
      return true;
    case engine_validator_checkDhtServers::ID:
      func(create_tl_object<engine_validator_checkDhtServers>());
      return true;
    case engine_validator_controlQuery::ID:
      func(create_tl_object<engine_validator_controlQuery>());
      return true;
    case engine_validator_createComplaintVote::ID:
      func(create_tl_object<engine_validator_createComplaintVote>());
      return true;
    case engine_validator_createElectionBid::ID:
      func(create_tl_object<engine_validator_createElectionBid>());
      return true;
    case engine_validator_createProposalVote::ID:
      func(create_tl_object<engine_validator_createProposalVote>());
      return true;
    case engine_validator_delAdnlId::ID:
      func(create_tl_object<engine_validator_delAdnlId>());
      return true;
    case engine_validator_delDhtId::ID:
      func(create_tl_object<engine_validator_delDhtId>());
      return true;
    case engine_validator_delListeningPort::ID:
      func(create_tl_object<engine_validator_delListeningPort>());
      return true;
    case engine_validator_delProxy::ID:
      func(create_tl_object<engine_validator_delProxy>());
      return true;
    case engine_validator_delValidatorAdnlAddress::ID:
      func(create_tl_object<engine_validator_delValidatorAdnlAddress>());
      return true;
    case engine_validator_delValidatorPermanentKey::ID:
      func(create_tl_object<engine_validator_delValidatorPermanentKey>());
      return true;
    case engine_validator_delValidatorTempKey::ID:
      func(create_tl_object<engine_validator_delValidatorTempKey>());
      return true;
    case engine_validator_exportPrivateKey::ID:
      func(create_tl_object<engine_validator_exportPrivateKey>());
      return true;
    case engine_validator_exportPublicKey::ID:
      func(create_tl_object<engine_validator_exportPublicKey>());
      return true;
    case engine_validator_generateKeyPair::ID:
      func(create_tl_object<engine_validator_generateKeyPair>());
      return true;
    case engine_validator_getConfig::ID:
      func(create_tl_object<engine_validator_getConfig>());
      return true;
    case engine_validator_getOverlaysStats::ID:
      func(create_tl_object<engine_validator_getOverlaysStats>());
      return true;
    case engine_validator_getPerfTimerStats::ID:
      func(create_tl_object<engine_validator_getPerfTimerStats>());
      return true;
    case engine_validator_getStats::ID:
      func(create_tl_object<engine_validator_getStats>());
      return true;
    case engine_validator_getTime::ID:
      func(create_tl_object<engine_validator_getTime>());
      return true;
    case engine_validator_importCertificate::ID:
      func(create_tl_object<engine_validator_importCertificate>());
      return true;
    case engine_validator_importPrivateKey::ID:
      func(create_tl_object<engine_validator_importPrivateKey>());
      return true;
    case engine_validator_importShardOverlayCertificate::ID:
      func(create_tl_object<engine_validator_importShardOverlayCertificate>());
      return true;
    case engine_validator_setVerbosity::ID:
      func(create_tl_object<engine_validator_setVerbosity>());
      return true;
    case engine_validator_sign::ID:
      func(create_tl_object<engine_validator_sign>());
      return true;
    case engine_validator_signShardOverlayCertificate::ID:
      func(create_tl_object<engine_validator_signShardOverlayCertificate>());
      return true;
    case getTestObject::ID:
      func(create_tl_object<getTestObject>());
      return true;
    case http_getNextPayloadPart::ID:
      func(create_tl_object<http_getNextPayloadPart>());
      return true;
    case http_proxy_getCapabilities::ID:
      func(create_tl_object<http_proxy_getCapabilities>());
      return true;
    case http_request::ID:
      func(create_tl_object<http_request>());
      return true;
    case overlay_getBroadcast::ID:
      func(create_tl_object<overlay_getBroadcast>());
      return true;
    case overlay_getBroadcastList::ID:
      func(create_tl_object<overlay_getBroadcastList>());
      return true;
    case overlay_getRandomPeers::ID:
      func(create_tl_object<overlay_getRandomPeers>());
      return true;
    case overlay_query::ID:
      func(create_tl_object<overlay_query>());
      return true;
    case storage_addUpdate::ID:
      func(create_tl_object<storage_addUpdate>());
      return true;
    case storage_daemon_addByHash::ID:
      func(create_tl_object<storage_daemon_addByHash>());
      return true;
    case storage_daemon_addByMeta::ID:
      func(create_tl_object<storage_daemon_addByMeta>());
      return true;
    case storage_daemon_closeStorageContract::ID:
      func(create_tl_object<storage_daemon_closeStorageContract>());
      return true;
    case storage_daemon_createTorrent::ID:
      func(create_tl_object<storage_daemon_createTorrent>());
      return true;
    case storage_daemon_deployProvider::ID:
      func(create_tl_object<storage_daemon_deployProvider>());
      return true;
    case storage_daemon_getNewContractMessage::ID:
      func(create_tl_object<storage_daemon_getNewContractMessage>());
      return true;
    case storage_daemon_getProviderInfo::ID:
      func(create_tl_object<storage_daemon_getProviderInfo>());
      return true;
    case storage_daemon_getProviderParams::ID:
      func(create_tl_object<storage_daemon_getProviderParams>());
      return true;
    case storage_daemon_getTorrentFull::ID:
      func(create_tl_object<storage_daemon_getTorrentFull>());
      return true;
    case storage_daemon_getTorrentMeta::ID:
      func(create_tl_object<storage_daemon_getTorrentMeta>());
      return true;
    case storage_daemon_getTorrentPeers::ID:
      func(create_tl_object<storage_daemon_getTorrentPeers>());
      return true;
    case storage_daemon_getTorrents::ID:
      func(create_tl_object<storage_daemon_getTorrents>());
      return true;
    case storage_daemon_importPrivateKey::ID:
      func(create_tl_object<storage_daemon_importPrivateKey>());
      return true;
    case storage_daemon_initProvider::ID:
      func(create_tl_object<storage_daemon_initProvider>());
      return true;
    case storage_daemon_loadFrom::ID:
      func(create_tl_object<storage_daemon_loadFrom>());
      return true;
    case storage_daemon_removeStorageProvider::ID:
      func(create_tl_object<storage_daemon_removeStorageProvider>());
      return true;
    case storage_daemon_removeTorrent::ID:
      func(create_tl_object<storage_daemon_removeTorrent>());
      return true;
    case storage_daemon_sendCoins::ID:
      func(create_tl_object<storage_daemon_sendCoins>());
      return true;
    case storage_daemon_setActiveDownload::ID:
      func(create_tl_object<storage_daemon_setActiveDownload>());
      return true;
    case storage_daemon_setActiveUpload::ID:
      func(create_tl_object<storage_daemon_setActiveUpload>());
      return true;
    case storage_daemon_setFilePriorityAll::ID:
      func(create_tl_object<storage_daemon_setFilePriorityAll>());
      return true;
    case storage_daemon_setFilePriorityByIdx::ID:
      func(create_tl_object<storage_daemon_setFilePriorityByIdx>());
      return true;
    case storage_daemon_setFilePriorityByName::ID:
      func(create_tl_object<storage_daemon_setFilePriorityByName>());
      return true;
    case storage_daemon_setProviderConfig::ID:
      func(create_tl_object<storage_daemon_setProviderConfig>());
      return true;
    case storage_daemon_setProviderParams::ID:
      func(create_tl_object<storage_daemon_setProviderParams>());
      return true;
    case storage_daemon_setVerbosity::ID:
      func(create_tl_object<storage_daemon_setVerbosity>());
      return true;
    case storage_daemon_withdraw::ID:
      func(create_tl_object<storage_daemon_withdraw>());
      return true;
    case storage_getPiece::ID:
      func(create_tl_object<storage_getPiece>());
      return true;
    case storage_getTorrentInfo::ID:
      func(create_tl_object<storage_getTorrentInfo>());
      return true;
    case storage_ping::ID:
      func(create_tl_object<storage_ping>());
      return true;
    case tcp_ping::ID:
      func(create_tl_object<tcp_ping>());
      return true;
    case tonNode_downloadBlock::ID:
      func(create_tl_object<tonNode_downloadBlock>());
      return true;
    case tonNode_downloadBlockFull::ID:
      func(create_tl_object<tonNode_downloadBlockFull>());
      return true;
    case tonNode_downloadBlockProof::ID:
      func(create_tl_object<tonNode_downloadBlockProof>());
      return true;
    case tonNode_downloadBlockProofLink::ID:
      func(create_tl_object<tonNode_downloadBlockProofLink>());
      return true;
    case tonNode_downloadBlockProofLinks::ID:
      func(create_tl_object<tonNode_downloadBlockProofLinks>());
      return true;
    case tonNode_downloadBlockProofs::ID:
      func(create_tl_object<tonNode_downloadBlockProofs>());
      return true;
    case tonNode_downloadBlocks::ID:
      func(create_tl_object<tonNode_downloadBlocks>());
      return true;
    case tonNode_downloadKeyBlockProof::ID:
      func(create_tl_object<tonNode_downloadKeyBlockProof>());
      return true;
    case tonNode_downloadKeyBlockProofLink::ID:
      func(create_tl_object<tonNode_downloadKeyBlockProofLink>());
      return true;
    case tonNode_downloadKeyBlockProofLinks::ID:
      func(create_tl_object<tonNode_downloadKeyBlockProofLinks>());
      return true;
    case tonNode_downloadKeyBlockProofs::ID:
      func(create_tl_object<tonNode_downloadKeyBlockProofs>());
      return true;
    case tonNode_downloadNextBlockFull::ID:
      func(create_tl_object<tonNode_downloadNextBlockFull>());
      return true;
    case tonNode_downloadPersistentState::ID:
      func(create_tl_object<tonNode_downloadPersistentState>());
      return true;
    case tonNode_downloadPersistentStateSlice::ID:
      func(create_tl_object<tonNode_downloadPersistentStateSlice>());
      return true;
    case tonNode_downloadZeroState::ID:
      func(create_tl_object<tonNode_downloadZeroState>());
      return true;
    case tonNode_getArchiveInfo::ID:
      func(create_tl_object<tonNode_getArchiveInfo>());
      return true;
    case tonNode_getArchiveSlice::ID:
      func(create_tl_object<tonNode_getArchiveSlice>());
      return true;
    case tonNode_getCapabilities::ID:
      func(create_tl_object<tonNode_getCapabilities>());
      return true;
    case tonNode_getNextBlockDescription::ID:
      func(create_tl_object<tonNode_getNextBlockDescription>());
      return true;
    case tonNode_getNextBlocksDescription::ID:
      func(create_tl_object<tonNode_getNextBlocksDescription>());
      return true;
    case tonNode_getNextKeyBlockIds::ID:
      func(create_tl_object<tonNode_getNextKeyBlockIds>());
      return true;
    case tonNode_getPrevBlocksDescription::ID:
      func(create_tl_object<tonNode_getPrevBlocksDescription>());
      return true;
    case tonNode_prepareBlock::ID:
      func(create_tl_object<tonNode_prepareBlock>());
      return true;
    case tonNode_prepareBlockProof::ID:
      func(create_tl_object<tonNode_prepareBlockProof>());
      return true;
    case tonNode_prepareBlockProofs::ID:
      func(create_tl_object<tonNode_prepareBlockProofs>());
      return true;
    case tonNode_prepareBlocks::ID:
      func(create_tl_object<tonNode_prepareBlocks>());
      return true;
    case tonNode_prepareKeyBlockProof::ID:
      func(create_tl_object<tonNode_prepareKeyBlockProof>());
      return true;
    case tonNode_prepareKeyBlockProofs::ID:
      func(create_tl_object<tonNode_prepareKeyBlockProofs>());
      return true;
    case tonNode_preparePersistentState::ID:
      func(create_tl_object<tonNode_preparePersistentState>());
      return true;
    case tonNode_prepareZeroState::ID:
      func(create_tl_object<tonNode_prepareZeroState>());
      return true;
    case tonNode_query::ID:
      func(create_tl_object<tonNode_query>());
      return true;
    case tonNode_slave_sendExtMessage::ID:
      func(create_tl_object<tonNode_slave_sendExtMessage>());
      return true;
    case validatorSession_downloadCandidate::ID:
      func(create_tl_object<validatorSession_downloadCandidate>());
      return true;
    case validatorSession_ping::ID:
      func(create_tl_object<validatorSession_ping>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(Hashable &obj, const T &func) {
  switch (obj.get_id()) {
    case hashable_bool::ID:
      func(static_cast<hashable_bool &>(obj));
      return true;
    case hashable_int32::ID:
      func(static_cast<hashable_int32 &>(obj));
      return true;
    case hashable_int64::ID:
      func(static_cast<hashable_int64 &>(obj));
      return true;
    case hashable_int256::ID:
      func(static_cast<hashable_int256 &>(obj));
      return true;
    case hashable_bytes::ID:
      func(static_cast<hashable_bytes &>(obj));
      return true;
    case hashable_pair::ID:
      func(static_cast<hashable_pair &>(obj));
      return true;
    case hashable_vector::ID:
      func(static_cast<hashable_vector &>(obj));
      return true;
    case hashable_validatorSessionOldRound::ID:
      func(static_cast<hashable_validatorSessionOldRound &>(obj));
      return true;
    case hashable_validatorSessionRoundAttempt::ID:
      func(static_cast<hashable_validatorSessionRoundAttempt &>(obj));
      return true;
    case hashable_validatorSessionRound::ID:
      func(static_cast<hashable_validatorSessionRound &>(obj));
      return true;
    case hashable_blockSignature::ID:
      func(static_cast<hashable_blockSignature &>(obj));
      return true;
    case hashable_sentBlock::ID:
      func(static_cast<hashable_sentBlock &>(obj));
      return true;
    case hashable_sentBlockEmpty::ID:
      func(static_cast<hashable_sentBlockEmpty &>(obj));
      return true;
    case hashable_vote::ID:
      func(static_cast<hashable_vote &>(obj));
      return true;
    case hashable_blockCandidate::ID:
      func(static_cast<hashable_blockCandidate &>(obj));
      return true;
    case hashable_blockVoteCandidate::ID:
      func(static_cast<hashable_blockVoteCandidate &>(obj));
      return true;
    case hashable_blockCandidateAttempt::ID:
      func(static_cast<hashable_blockCandidateAttempt &>(obj));
      return true;
    case hashable_cntVector::ID:
      func(static_cast<hashable_cntVector &>(obj));
      return true;
    case hashable_cntSortedVector::ID:
      func(static_cast<hashable_cntSortedVector &>(obj));
      return true;
    case hashable_validatorSession::ID:
      func(static_cast<hashable_validatorSession &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(Hashable &obj, const T &func) {
switch (obj.get_id()) {    case hashable_bool::ID:
      func(create_tl_object<hashable_bool>());
      return true;
    case hashable_int32::ID:
      func(create_tl_object<hashable_int32>());
      return true;
    case hashable_int64::ID:
      func(create_tl_object<hashable_int64>());
      return true;
    case hashable_int256::ID:
      func(create_tl_object<hashable_int256>());
      return true;
    case hashable_bytes::ID:
      func(create_tl_object<hashable_bytes>());
      return true;
    case hashable_pair::ID:
      func(create_tl_object<hashable_pair>());
      return true;
    case hashable_vector::ID:
      func(create_tl_object<hashable_vector>());
      return true;
    case hashable_validatorSessionOldRound::ID:
      func(create_tl_object<hashable_validatorSessionOldRound>());
      return true;
    case hashable_validatorSessionRoundAttempt::ID:
      func(create_tl_object<hashable_validatorSessionRoundAttempt>());
      return true;
    case hashable_validatorSessionRound::ID:
      func(create_tl_object<hashable_validatorSessionRound>());
      return true;
    case hashable_blockSignature::ID:
      func(create_tl_object<hashable_blockSignature>());
      return true;
    case hashable_sentBlock::ID:
      func(create_tl_object<hashable_sentBlock>());
      return true;
    case hashable_sentBlockEmpty::ID:
      func(create_tl_object<hashable_sentBlockEmpty>());
      return true;
    case hashable_vote::ID:
      func(create_tl_object<hashable_vote>());
      return true;
    case hashable_blockCandidate::ID:
      func(create_tl_object<hashable_blockCandidate>());
      return true;
    case hashable_blockVoteCandidate::ID:
      func(create_tl_object<hashable_blockVoteCandidate>());
      return true;
    case hashable_blockCandidateAttempt::ID:
      func(create_tl_object<hashable_blockCandidateAttempt>());
      return true;
    case hashable_cntVector::ID:
      func(create_tl_object<hashable_cntVector>());
      return true;
    case hashable_cntSortedVector::ID:
      func(create_tl_object<hashable_cntSortedVector>());
      return true;
    case hashable_validatorSession::ID:
      func(create_tl_object<hashable_validatorSession>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(PrivateKey &obj, const T &func) {
  switch (obj.get_id()) {
    case pk_unenc::ID:
      func(static_cast<pk_unenc &>(obj));
      return true;
    case pk_ed25519::ID:
      func(static_cast<pk_ed25519 &>(obj));
      return true;
    case pk_aes::ID:
      func(static_cast<pk_aes &>(obj));
      return true;
    case pk_overlay::ID:
      func(static_cast<pk_overlay &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(PrivateKey &obj, const T &func) {
switch (obj.get_id()) {    case pk_unenc::ID:
      func(create_tl_object<pk_unenc>());
      return true;
    case pk_ed25519::ID:
      func(create_tl_object<pk_ed25519>());
      return true;
    case pk_aes::ID:
      func(create_tl_object<pk_aes>());
      return true;
    case pk_overlay::ID:
      func(create_tl_object<pk_overlay>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(PublicKey &obj, const T &func) {
  switch (obj.get_id()) {
    case pub_unenc::ID:
      func(static_cast<pub_unenc &>(obj));
      return true;
    case pub_ed25519::ID:
      func(static_cast<pub_ed25519 &>(obj));
      return true;
    case pub_aes::ID:
      func(static_cast<pub_aes &>(obj));
      return true;
    case pub_overlay::ID:
      func(static_cast<pub_overlay &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(PublicKey &obj, const T &func) {
switch (obj.get_id()) {    case pub_unenc::ID:
      func(create_tl_object<pub_unenc>());
      return true;
    case pub_ed25519::ID:
      func(create_tl_object<pub_ed25519>());
      return true;
    case pub_aes::ID:
      func(create_tl_object<pub_aes>());
      return true;
    case pub_overlay::ID:
      func(create_tl_object<pub_overlay>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(TestObject &obj, const T &func) {
  switch (obj.get_id()) {
    case testObject::ID:
      func(static_cast<testObject &>(obj));
      return true;
    case testString::ID:
      func(static_cast<testString &>(obj));
      return true;
    case testInt::ID:
      func(static_cast<testInt &>(obj));
      return true;
    case testVectorBytes::ID:
      func(static_cast<testVectorBytes &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(TestObject &obj, const T &func) {
switch (obj.get_id()) {    case testObject::ID:
      func(create_tl_object<testObject>());
      return true;
    case testString::ID:
      func(create_tl_object<testString>());
      return true;
    case testInt::ID:
      func(create_tl_object<testInt>());
      return true;
    case testVectorBytes::ID:
      func(create_tl_object<testVectorBytes>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(adnl_Address &obj, const T &func) {
  switch (obj.get_id()) {
    case adnl_address_udp::ID:
      func(static_cast<adnl_address_udp &>(obj));
      return true;
    case adnl_address_udp6::ID:
      func(static_cast<adnl_address_udp6 &>(obj));
      return true;
    case adnl_address_tunnel::ID:
      func(static_cast<adnl_address_tunnel &>(obj));
      return true;
    case adnl_address_reverse::ID:
      func(static_cast<adnl_address_reverse &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(adnl_Address &obj, const T &func) {
switch (obj.get_id()) {    case adnl_address_udp::ID:
      func(create_tl_object<adnl_address_udp>());
      return true;
    case adnl_address_udp6::ID:
      func(create_tl_object<adnl_address_udp6>());
      return true;
    case adnl_address_tunnel::ID:
      func(create_tl_object<adnl_address_tunnel>());
      return true;
    case adnl_address_reverse::ID:
      func(create_tl_object<adnl_address_reverse>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(adnl_Message &obj, const T &func) {
  switch (obj.get_id()) {
    case adnl_message_createChannel::ID:
      func(static_cast<adnl_message_createChannel &>(obj));
      return true;
    case adnl_message_confirmChannel::ID:
      func(static_cast<adnl_message_confirmChannel &>(obj));
      return true;
    case adnl_message_custom::ID:
      func(static_cast<adnl_message_custom &>(obj));
      return true;
    case adnl_message_nop::ID:
      func(static_cast<adnl_message_nop &>(obj));
      return true;
    case adnl_message_reinit::ID:
      func(static_cast<adnl_message_reinit &>(obj));
      return true;
    case adnl_message_query::ID:
      func(static_cast<adnl_message_query &>(obj));
      return true;
    case adnl_message_answer::ID:
      func(static_cast<adnl_message_answer &>(obj));
      return true;
    case adnl_message_part::ID:
      func(static_cast<adnl_message_part &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(adnl_Message &obj, const T &func) {
switch (obj.get_id()) {    case adnl_message_createChannel::ID:
      func(create_tl_object<adnl_message_createChannel>());
      return true;
    case adnl_message_confirmChannel::ID:
      func(create_tl_object<adnl_message_confirmChannel>());
      return true;
    case adnl_message_custom::ID:
      func(create_tl_object<adnl_message_custom>());
      return true;
    case adnl_message_nop::ID:
      func(create_tl_object<adnl_message_nop>());
      return true;
    case adnl_message_reinit::ID:
      func(create_tl_object<adnl_message_reinit>());
      return true;
    case adnl_message_query::ID:
      func(create_tl_object<adnl_message_query>());
      return true;
    case adnl_message_answer::ID:
      func(create_tl_object<adnl_message_answer>());
      return true;
    case adnl_message_part::ID:
      func(create_tl_object<adnl_message_part>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(adnl_Proxy &obj, const T &func) {
  switch (obj.get_id()) {
    case adnl_proxy_none::ID:
      func(static_cast<adnl_proxy_none &>(obj));
      return true;
    case adnl_proxy_fast::ID:
      func(static_cast<adnl_proxy_fast &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(adnl_Proxy &obj, const T &func) {
switch (obj.get_id()) {    case adnl_proxy_none::ID:
      func(create_tl_object<adnl_proxy_none>());
      return true;
    case adnl_proxy_fast::ID:
      func(create_tl_object<adnl_proxy_fast>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(adnl_ProxyControlPacket &obj, const T &func) {
  switch (obj.get_id()) {
    case adnl_proxyControlPacketPing::ID:
      func(static_cast<adnl_proxyControlPacketPing &>(obj));
      return true;
    case adnl_proxyControlPacketPong::ID:
      func(static_cast<adnl_proxyControlPacketPong &>(obj));
      return true;
    case adnl_proxyControlPacketRegister::ID:
      func(static_cast<adnl_proxyControlPacketRegister &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(adnl_ProxyControlPacket &obj, const T &func) {
switch (obj.get_id()) {    case adnl_proxyControlPacketPing::ID:
      func(create_tl_object<adnl_proxyControlPacketPing>());
      return true;
    case adnl_proxyControlPacketPong::ID:
      func(create_tl_object<adnl_proxyControlPacketPong>());
      return true;
    case adnl_proxyControlPacketRegister::ID:
      func(create_tl_object<adnl_proxyControlPacketRegister>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(catchain_BlockResult &obj, const T &func) {
  switch (obj.get_id()) {
    case catchain_blockNotFound::ID:
      func(static_cast<catchain_blockNotFound &>(obj));
      return true;
    case catchain_blockResult::ID:
      func(static_cast<catchain_blockResult &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(catchain_BlockResult &obj, const T &func) {
switch (obj.get_id()) {    case catchain_blockNotFound::ID:
      func(create_tl_object<catchain_blockNotFound>());
      return true;
    case catchain_blockResult::ID:
      func(create_tl_object<catchain_blockResult>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(catchain_Difference &obj, const T &func) {
  switch (obj.get_id()) {
    case catchain_difference::ID:
      func(static_cast<catchain_difference &>(obj));
      return true;
    case catchain_differenceFork::ID:
      func(static_cast<catchain_differenceFork &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(catchain_Difference &obj, const T &func) {
switch (obj.get_id()) {    case catchain_difference::ID:
      func(create_tl_object<catchain_difference>());
      return true;
    case catchain_differenceFork::ID:
      func(create_tl_object<catchain_differenceFork>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(catchain_block_inner_Data &obj, const T &func) {
  switch (obj.get_id()) {
    case catchain_block_data_badBlock::ID:
      func(static_cast<catchain_block_data_badBlock &>(obj));
      return true;
    case catchain_block_data_fork::ID:
      func(static_cast<catchain_block_data_fork &>(obj));
      return true;
    case catchain_block_data_nop::ID:
      func(static_cast<catchain_block_data_nop &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(catchain_block_inner_Data &obj, const T &func) {
switch (obj.get_id()) {    case catchain_block_data_badBlock::ID:
      func(create_tl_object<catchain_block_data_badBlock>());
      return true;
    case catchain_block_data_fork::ID:
      func(create_tl_object<catchain_block_data_fork>());
      return true;
    case catchain_block_data_nop::ID:
      func(create_tl_object<catchain_block_data_nop>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_block_Info &obj, const T &func) {
  switch (obj.get_id()) {
    case db_block_info::ID:
      func(static_cast<db_block_info &>(obj));
      return true;
    case db_block_packedInfo::ID:
      func(static_cast<db_block_packedInfo &>(obj));
      return true;
    case db_block_archivedInfo::ID:
      func(static_cast<db_block_archivedInfo &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_block_Info &obj, const T &func) {
switch (obj.get_id()) {    case db_block_info::ID:
      func(create_tl_object<db_block_info>());
      return true;
    case db_block_packedInfo::ID:
      func(create_tl_object<db_block_packedInfo>());
      return true;
    case db_block_archivedInfo::ID:
      func(create_tl_object<db_block_archivedInfo>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_blockdb_Key &obj, const T &func) {
  switch (obj.get_id()) {
    case db_blockdb_key_lru::ID:
      func(static_cast<db_blockdb_key_lru &>(obj));
      return true;
    case db_blockdb_key_value::ID:
      func(static_cast<db_blockdb_key_value &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_blockdb_Key &obj, const T &func) {
switch (obj.get_id()) {    case db_blockdb_key_lru::ID:
      func(create_tl_object<db_blockdb_key_lru>());
      return true;
    case db_blockdb_key_value::ID:
      func(create_tl_object<db_blockdb_key_value>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_filedb_Key &obj, const T &func) {
  switch (obj.get_id()) {
    case db_filedb_key_empty::ID:
      func(static_cast<db_filedb_key_empty &>(obj));
      return true;
    case db_filedb_key_blockFile::ID:
      func(static_cast<db_filedb_key_blockFile &>(obj));
      return true;
    case db_filedb_key_zeroStateFile::ID:
      func(static_cast<db_filedb_key_zeroStateFile &>(obj));
      return true;
    case db_filedb_key_persistentStateFile::ID:
      func(static_cast<db_filedb_key_persistentStateFile &>(obj));
      return true;
    case db_filedb_key_proof::ID:
      func(static_cast<db_filedb_key_proof &>(obj));
      return true;
    case db_filedb_key_proofLink::ID:
      func(static_cast<db_filedb_key_proofLink &>(obj));
      return true;
    case db_filedb_key_signatures::ID:
      func(static_cast<db_filedb_key_signatures &>(obj));
      return true;
    case db_filedb_key_candidate::ID:
      func(static_cast<db_filedb_key_candidate &>(obj));
      return true;
    case db_filedb_key_blockInfo::ID:
      func(static_cast<db_filedb_key_blockInfo &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_filedb_Key &obj, const T &func) {
switch (obj.get_id()) {    case db_filedb_key_empty::ID:
      func(create_tl_object<db_filedb_key_empty>());
      return true;
    case db_filedb_key_blockFile::ID:
      func(create_tl_object<db_filedb_key_blockFile>());
      return true;
    case db_filedb_key_zeroStateFile::ID:
      func(create_tl_object<db_filedb_key_zeroStateFile>());
      return true;
    case db_filedb_key_persistentStateFile::ID:
      func(create_tl_object<db_filedb_key_persistentStateFile>());
      return true;
    case db_filedb_key_proof::ID:
      func(create_tl_object<db_filedb_key_proof>());
      return true;
    case db_filedb_key_proofLink::ID:
      func(create_tl_object<db_filedb_key_proofLink>());
      return true;
    case db_filedb_key_signatures::ID:
      func(create_tl_object<db_filedb_key_signatures>());
      return true;
    case db_filedb_key_candidate::ID:
      func(create_tl_object<db_filedb_key_candidate>());
      return true;
    case db_filedb_key_blockInfo::ID:
      func(create_tl_object<db_filedb_key_blockInfo>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_files_Key &obj, const T &func) {
  switch (obj.get_id()) {
    case db_files_index_key::ID:
      func(static_cast<db_files_index_key &>(obj));
      return true;
    case db_files_package_key::ID:
      func(static_cast<db_files_package_key &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_files_Key &obj, const T &func) {
switch (obj.get_id()) {    case db_files_index_key::ID:
      func(create_tl_object<db_files_index_key>());
      return true;
    case db_files_package_key::ID:
      func(create_tl_object<db_files_package_key>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_lt_Key &obj, const T &func) {
  switch (obj.get_id()) {
    case db_lt_el_key::ID:
      func(static_cast<db_lt_el_key &>(obj));
      return true;
    case db_lt_desc_key::ID:
      func(static_cast<db_lt_desc_key &>(obj));
      return true;
    case db_lt_shard_key::ID:
      func(static_cast<db_lt_shard_key &>(obj));
      return true;
    case db_lt_status_key::ID:
      func(static_cast<db_lt_status_key &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_lt_Key &obj, const T &func) {
switch (obj.get_id()) {    case db_lt_el_key::ID:
      func(create_tl_object<db_lt_el_key>());
      return true;
    case db_lt_desc_key::ID:
      func(create_tl_object<db_lt_desc_key>());
      return true;
    case db_lt_shard_key::ID:
      func(create_tl_object<db_lt_shard_key>());
      return true;
    case db_lt_status_key::ID:
      func(create_tl_object<db_lt_status_key>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_root_Key &obj, const T &func) {
  switch (obj.get_id()) {
    case db_root_key_cellDb::ID:
      func(static_cast<db_root_key_cellDb &>(obj));
      return true;
    case db_root_key_blockDb::ID:
      func(static_cast<db_root_key_blockDb &>(obj));
      return true;
    case db_root_key_config::ID:
      func(static_cast<db_root_key_config &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_root_Key &obj, const T &func) {
switch (obj.get_id()) {    case db_root_key_cellDb::ID:
      func(create_tl_object<db_root_key_cellDb>());
      return true;
    case db_root_key_blockDb::ID:
      func(create_tl_object<db_root_key_blockDb>());
      return true;
    case db_root_key_config::ID:
      func(create_tl_object<db_root_key_config>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(db_state_Key &obj, const T &func) {
  switch (obj.get_id()) {
    case db_state_key_destroyedSessions::ID:
      func(static_cast<db_state_key_destroyedSessions &>(obj));
      return true;
    case db_state_key_initBlockId::ID:
      func(static_cast<db_state_key_initBlockId &>(obj));
      return true;
    case db_state_key_gcBlockId::ID:
      func(static_cast<db_state_key_gcBlockId &>(obj));
      return true;
    case db_state_key_shardClient::ID:
      func(static_cast<db_state_key_shardClient &>(obj));
      return true;
    case db_state_key_asyncSerializer::ID:
      func(static_cast<db_state_key_asyncSerializer &>(obj));
      return true;
    case db_state_key_hardforks::ID:
      func(static_cast<db_state_key_hardforks &>(obj));
      return true;
    case db_state_key_dbVersion::ID:
      func(static_cast<db_state_key_dbVersion &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(db_state_Key &obj, const T &func) {
switch (obj.get_id()) {    case db_state_key_destroyedSessions::ID:
      func(create_tl_object<db_state_key_destroyedSessions>());
      return true;
    case db_state_key_initBlockId::ID:
      func(create_tl_object<db_state_key_initBlockId>());
      return true;
    case db_state_key_gcBlockId::ID:
      func(create_tl_object<db_state_key_gcBlockId>());
      return true;
    case db_state_key_shardClient::ID:
      func(create_tl_object<db_state_key_shardClient>());
      return true;
    case db_state_key_asyncSerializer::ID:
      func(create_tl_object<db_state_key_asyncSerializer>());
      return true;
    case db_state_key_hardforks::ID:
      func(create_tl_object<db_state_key_hardforks>());
      return true;
    case db_state_key_dbVersion::ID:
      func(create_tl_object<db_state_key_dbVersion>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(dht_ReversePingResult &obj, const T &func) {
  switch (obj.get_id()) {
    case dht_clientNotFound::ID:
      func(static_cast<dht_clientNotFound &>(obj));
      return true;
    case dht_reversePingOk::ID:
      func(static_cast<dht_reversePingOk &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(dht_ReversePingResult &obj, const T &func) {
switch (obj.get_id()) {    case dht_clientNotFound::ID:
      func(create_tl_object<dht_clientNotFound>());
      return true;
    case dht_reversePingOk::ID:
      func(create_tl_object<dht_reversePingOk>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(dht_UpdateRule &obj, const T &func) {
  switch (obj.get_id()) {
    case dht_updateRule_signature::ID:
      func(static_cast<dht_updateRule_signature &>(obj));
      return true;
    case dht_updateRule_anybody::ID:
      func(static_cast<dht_updateRule_anybody &>(obj));
      return true;
    case dht_updateRule_overlayNodes::ID:
      func(static_cast<dht_updateRule_overlayNodes &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(dht_UpdateRule &obj, const T &func) {
switch (obj.get_id()) {    case dht_updateRule_signature::ID:
      func(create_tl_object<dht_updateRule_signature>());
      return true;
    case dht_updateRule_anybody::ID:
      func(create_tl_object<dht_updateRule_anybody>());
      return true;
    case dht_updateRule_overlayNodes::ID:
      func(create_tl_object<dht_updateRule_overlayNodes>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(dht_ValueResult &obj, const T &func) {
  switch (obj.get_id()) {
    case dht_valueNotFound::ID:
      func(static_cast<dht_valueNotFound &>(obj));
      return true;
    case dht_valueFound::ID:
      func(static_cast<dht_valueFound &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(dht_ValueResult &obj, const T &func) {
switch (obj.get_id()) {    case dht_valueNotFound::ID:
      func(create_tl_object<dht_valueNotFound>());
      return true;
    case dht_valueFound::ID:
      func(create_tl_object<dht_valueFound>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(dht_config_Global &obj, const T &func) {
  switch (obj.get_id()) {
    case dht_config_global::ID:
      func(static_cast<dht_config_global &>(obj));
      return true;
    case dht_config_global_v2::ID:
      func(static_cast<dht_config_global_v2 &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(dht_config_Global &obj, const T &func) {
switch (obj.get_id()) {    case dht_config_global::ID:
      func(create_tl_object<dht_config_global>());
      return true;
    case dht_config_global_v2::ID:
      func(create_tl_object<dht_config_global_v2>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(dht_config_Local &obj, const T &func) {
  switch (obj.get_id()) {
    case dht_config_local::ID:
      func(static_cast<dht_config_local &>(obj));
      return true;
    case dht_config_random_local::ID:
      func(static_cast<dht_config_random_local &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(dht_config_Local &obj, const T &func) {
switch (obj.get_id()) {    case dht_config_local::ID:
      func(create_tl_object<dht_config_local>());
      return true;
    case dht_config_random_local::ID:
      func(create_tl_object<dht_config_random_local>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(engine_Addr &obj, const T &func) {
  switch (obj.get_id()) {
    case engine_addr::ID:
      func(static_cast<engine_addr &>(obj));
      return true;
    case engine_addrProxy::ID:
      func(static_cast<engine_addrProxy &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(engine_Addr &obj, const T &func) {
switch (obj.get_id()) {    case engine_addr::ID:
      func(create_tl_object<engine_addr>());
      return true;
    case engine_addrProxy::ID:
      func(create_tl_object<engine_addrProxy>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(fec_Type &obj, const T &func) {
  switch (obj.get_id()) {
    case fec_raptorQ::ID:
      func(static_cast<fec_raptorQ &>(obj));
      return true;
    case fec_roundRobin::ID:
      func(static_cast<fec_roundRobin &>(obj));
      return true;
    case fec_online::ID:
      func(static_cast<fec_online &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(fec_Type &obj, const T &func) {
switch (obj.get_id()) {    case fec_raptorQ::ID:
      func(create_tl_object<fec_raptorQ>());
      return true;
    case fec_roundRobin::ID:
      func(create_tl_object<fec_roundRobin>());
      return true;
    case fec_online::ID:
      func(create_tl_object<fec_online>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(liteserver_config_Local &obj, const T &func) {
  switch (obj.get_id()) {
    case liteserver_config_local::ID:
      func(static_cast<liteserver_config_local &>(obj));
      return true;
    case liteserver_config_random_local::ID:
      func(static_cast<liteserver_config_random_local &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(liteserver_config_Local &obj, const T &func) {
switch (obj.get_id()) {    case liteserver_config_local::ID:
      func(create_tl_object<liteserver_config_local>());
      return true;
    case liteserver_config_random_local::ID:
      func(create_tl_object<liteserver_config_random_local>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(overlay_Broadcast &obj, const T &func) {
  switch (obj.get_id()) {
    case overlay_fec_received::ID:
      func(static_cast<overlay_fec_received &>(obj));
      return true;
    case overlay_fec_completed::ID:
      func(static_cast<overlay_fec_completed &>(obj));
      return true;
    case overlay_unicast::ID:
      func(static_cast<overlay_unicast &>(obj));
      return true;
    case overlay_broadcast::ID:
      func(static_cast<overlay_broadcast &>(obj));
      return true;
    case overlay_broadcastFec::ID:
      func(static_cast<overlay_broadcastFec &>(obj));
      return true;
    case overlay_broadcastFecShort::ID:
      func(static_cast<overlay_broadcastFecShort &>(obj));
      return true;
    case overlay_broadcastNotFound::ID:
      func(static_cast<overlay_broadcastNotFound &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(overlay_Broadcast &obj, const T &func) {
switch (obj.get_id()) {    case overlay_fec_received::ID:
      func(create_tl_object<overlay_fec_received>());
      return true;
    case overlay_fec_completed::ID:
      func(create_tl_object<overlay_fec_completed>());
      return true;
    case overlay_unicast::ID:
      func(create_tl_object<overlay_unicast>());
      return true;
    case overlay_broadcast::ID:
      func(create_tl_object<overlay_broadcast>());
      return true;
    case overlay_broadcastFec::ID:
      func(create_tl_object<overlay_broadcastFec>());
      return true;
    case overlay_broadcastFecShort::ID:
      func(create_tl_object<overlay_broadcastFecShort>());
      return true;
    case overlay_broadcastNotFound::ID:
      func(create_tl_object<overlay_broadcastNotFound>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(overlay_Certificate &obj, const T &func) {
  switch (obj.get_id()) {
    case overlay_certificate::ID:
      func(static_cast<overlay_certificate &>(obj));
      return true;
    case overlay_certificateV2::ID:
      func(static_cast<overlay_certificateV2 &>(obj));
      return true;
    case overlay_emptyCertificate::ID:
      func(static_cast<overlay_emptyCertificate &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(overlay_Certificate &obj, const T &func) {
switch (obj.get_id()) {    case overlay_certificate::ID:
      func(create_tl_object<overlay_certificate>());
      return true;
    case overlay_certificateV2::ID:
      func(create_tl_object<overlay_certificateV2>());
      return true;
    case overlay_emptyCertificate::ID:
      func(create_tl_object<overlay_emptyCertificate>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(overlay_CertificateId &obj, const T &func) {
  switch (obj.get_id()) {
    case overlay_certificateId::ID:
      func(static_cast<overlay_certificateId &>(obj));
      return true;
    case overlay_certificateIdV2::ID:
      func(static_cast<overlay_certificateIdV2 &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(overlay_CertificateId &obj, const T &func) {
switch (obj.get_id()) {    case overlay_certificateId::ID:
      func(create_tl_object<overlay_certificateId>());
      return true;
    case overlay_certificateIdV2::ID:
      func(create_tl_object<overlay_certificateIdV2>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(rldp_Message &obj, const T &func) {
  switch (obj.get_id()) {
    case rldp_message::ID:
      func(static_cast<rldp_message &>(obj));
      return true;
    case rldp_query::ID:
      func(static_cast<rldp_query &>(obj));
      return true;
    case rldp_answer::ID:
      func(static_cast<rldp_answer &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(rldp_Message &obj, const T &func) {
switch (obj.get_id()) {    case rldp_message::ID:
      func(create_tl_object<rldp_message>());
      return true;
    case rldp_query::ID:
      func(create_tl_object<rldp_query>());
      return true;
    case rldp_answer::ID:
      func(create_tl_object<rldp_answer>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(rldp_MessagePart &obj, const T &func) {
  switch (obj.get_id()) {
    case rldp_messagePart::ID:
      func(static_cast<rldp_messagePart &>(obj));
      return true;
    case rldp_confirm::ID:
      func(static_cast<rldp_confirm &>(obj));
      return true;
    case rldp_complete::ID:
      func(static_cast<rldp_complete &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(rldp_MessagePart &obj, const T &func) {
switch (obj.get_id()) {    case rldp_messagePart::ID:
      func(create_tl_object<rldp_messagePart>());
      return true;
    case rldp_confirm::ID:
      func(create_tl_object<rldp_confirm>());
      return true;
    case rldp_complete::ID:
      func(create_tl_object<rldp_complete>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(rldp2_MessagePart &obj, const T &func) {
  switch (obj.get_id()) {
    case rldp2_messagePart::ID:
      func(static_cast<rldp2_messagePart &>(obj));
      return true;
    case rldp2_confirm::ID:
      func(static_cast<rldp2_confirm &>(obj));
      return true;
    case rldp2_complete::ID:
      func(static_cast<rldp2_complete &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(rldp2_MessagePart &obj, const T &func) {
switch (obj.get_id()) {    case rldp2_messagePart::ID:
      func(create_tl_object<rldp2_messagePart>());
      return true;
    case rldp2_confirm::ID:
      func(create_tl_object<rldp2_confirm>());
      return true;
    case rldp2_complete::ID:
      func(create_tl_object<rldp2_complete>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(storage_PriorityAction &obj, const T &func) {
  switch (obj.get_id()) {
    case storage_priorityAction_all::ID:
      func(static_cast<storage_priorityAction_all &>(obj));
      return true;
    case storage_priorityAction_idx::ID:
      func(static_cast<storage_priorityAction_idx &>(obj));
      return true;
    case storage_priorityAction_name::ID:
      func(static_cast<storage_priorityAction_name &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(storage_PriorityAction &obj, const T &func) {
switch (obj.get_id()) {    case storage_priorityAction_all::ID:
      func(create_tl_object<storage_priorityAction_all>());
      return true;
    case storage_priorityAction_idx::ID:
      func(create_tl_object<storage_priorityAction_idx>());
      return true;
    case storage_priorityAction_name::ID:
      func(create_tl_object<storage_priorityAction_name>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(storage_Update &obj, const T &func) {
  switch (obj.get_id()) {
    case storage_updateInit::ID:
      func(static_cast<storage_updateInit &>(obj));
      return true;
    case storage_updateHavePieces::ID:
      func(static_cast<storage_updateHavePieces &>(obj));
      return true;
    case storage_updateState::ID:
      func(static_cast<storage_updateState &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(storage_Update &obj, const T &func) {
switch (obj.get_id()) {    case storage_updateInit::ID:
      func(create_tl_object<storage_updateInit>());
      return true;
    case storage_updateHavePieces::ID:
      func(create_tl_object<storage_updateHavePieces>());
      return true;
    case storage_updateState::ID:
      func(create_tl_object<storage_updateState>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(storage_daemon_NewContractParams &obj, const T &func) {
  switch (obj.get_id()) {
    case storage_daemon_newContractParams::ID:
      func(static_cast<storage_daemon_newContractParams &>(obj));
      return true;
    case storage_daemon_newContractParamsAuto::ID:
      func(static_cast<storage_daemon_newContractParamsAuto &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(storage_daemon_NewContractParams &obj, const T &func) {
switch (obj.get_id()) {    case storage_daemon_newContractParams::ID:
      func(create_tl_object<storage_daemon_newContractParams>());
      return true;
    case storage_daemon_newContractParamsAuto::ID:
      func(create_tl_object<storage_daemon_newContractParamsAuto>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(storage_daemon_SetPriorityStatus &obj, const T &func) {
  switch (obj.get_id()) {
    case storage_daemon_prioritySet::ID:
      func(static_cast<storage_daemon_prioritySet &>(obj));
      return true;
    case storage_daemon_priorityPending::ID:
      func(static_cast<storage_daemon_priorityPending &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(storage_daemon_SetPriorityStatus &obj, const T &func) {
switch (obj.get_id()) {    case storage_daemon_prioritySet::ID:
      func(create_tl_object<storage_daemon_prioritySet>());
      return true;
    case storage_daemon_priorityPending::ID:
      func(create_tl_object<storage_daemon_priorityPending>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tcp_Message &obj, const T &func) {
  switch (obj.get_id()) {
    case tcp_authentificate::ID:
      func(static_cast<tcp_authentificate &>(obj));
      return true;
    case tcp_authentificationNonce::ID:
      func(static_cast<tcp_authentificationNonce &>(obj));
      return true;
    case tcp_authentificationComplete::ID:
      func(static_cast<tcp_authentificationComplete &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tcp_Message &obj, const T &func) {
switch (obj.get_id()) {    case tcp_authentificate::ID:
      func(create_tl_object<tcp_authentificate>());
      return true;
    case tcp_authentificationNonce::ID:
      func(create_tl_object<tcp_authentificationNonce>());
      return true;
    case tcp_authentificationComplete::ID:
      func(create_tl_object<tcp_authentificationComplete>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(ton_BlockId &obj, const T &func) {
  switch (obj.get_id()) {
    case ton_blockId::ID:
      func(static_cast<ton_blockId &>(obj));
      return true;
    case ton_blockIdApprove::ID:
      func(static_cast<ton_blockIdApprove &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(ton_BlockId &obj, const T &func) {
switch (obj.get_id()) {    case ton_blockId::ID:
      func(create_tl_object<ton_blockId>());
      return true;
    case ton_blockIdApprove::ID:
      func(create_tl_object<ton_blockIdApprove>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_ArchiveInfo &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_archiveNotFound::ID:
      func(static_cast<tonNode_archiveNotFound &>(obj));
      return true;
    case tonNode_archiveInfo::ID:
      func(static_cast<tonNode_archiveInfo &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_ArchiveInfo &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_archiveNotFound::ID:
      func(create_tl_object<tonNode_archiveNotFound>());
      return true;
    case tonNode_archiveInfo::ID:
      func(create_tl_object<tonNode_archiveInfo>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_BlockDescription &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_blockDescriptionEmpty::ID:
      func(static_cast<tonNode_blockDescriptionEmpty &>(obj));
      return true;
    case tonNode_blockDescription::ID:
      func(static_cast<tonNode_blockDescription &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_BlockDescription &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_blockDescriptionEmpty::ID:
      func(create_tl_object<tonNode_blockDescriptionEmpty>());
      return true;
    case tonNode_blockDescription::ID:
      func(create_tl_object<tonNode_blockDescription>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_Broadcast &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_blockBroadcast::ID:
      func(static_cast<tonNode_blockBroadcast &>(obj));
      return true;
    case tonNode_ihrMessageBroadcast::ID:
      func(static_cast<tonNode_ihrMessageBroadcast &>(obj));
      return true;
    case tonNode_externalMessageBroadcast::ID:
      func(static_cast<tonNode_externalMessageBroadcast &>(obj));
      return true;
    case tonNode_newShardBlockBroadcast::ID:
      func(static_cast<tonNode_newShardBlockBroadcast &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_Broadcast &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_blockBroadcast::ID:
      func(create_tl_object<tonNode_blockBroadcast>());
      return true;
    case tonNode_ihrMessageBroadcast::ID:
      func(create_tl_object<tonNode_ihrMessageBroadcast>());
      return true;
    case tonNode_externalMessageBroadcast::ID:
      func(create_tl_object<tonNode_externalMessageBroadcast>());
      return true;
    case tonNode_newShardBlockBroadcast::ID:
      func(create_tl_object<tonNode_newShardBlockBroadcast>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_DataFull &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_dataFull::ID:
      func(static_cast<tonNode_dataFull &>(obj));
      return true;
    case tonNode_dataFullEmpty::ID:
      func(static_cast<tonNode_dataFullEmpty &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_DataFull &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_dataFull::ID:
      func(create_tl_object<tonNode_dataFull>());
      return true;
    case tonNode_dataFullEmpty::ID:
      func(create_tl_object<tonNode_dataFullEmpty>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_Prepared &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_prepared::ID:
      func(static_cast<tonNode_prepared &>(obj));
      return true;
    case tonNode_notFound::ID:
      func(static_cast<tonNode_notFound &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_Prepared &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_prepared::ID:
      func(create_tl_object<tonNode_prepared>());
      return true;
    case tonNode_notFound::ID:
      func(create_tl_object<tonNode_notFound>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_PreparedProof &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_preparedProofEmpty::ID:
      func(static_cast<tonNode_preparedProofEmpty &>(obj));
      return true;
    case tonNode_preparedProof::ID:
      func(static_cast<tonNode_preparedProof &>(obj));
      return true;
    case tonNode_preparedProofLink::ID:
      func(static_cast<tonNode_preparedProofLink &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_PreparedProof &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_preparedProofEmpty::ID:
      func(create_tl_object<tonNode_preparedProofEmpty>());
      return true;
    case tonNode_preparedProof::ID:
      func(create_tl_object<tonNode_preparedProof>());
      return true;
    case tonNode_preparedProofLink::ID:
      func(create_tl_object<tonNode_preparedProofLink>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(tonNode_PreparedState &obj, const T &func) {
  switch (obj.get_id()) {
    case tonNode_preparedState::ID:
      func(static_cast<tonNode_preparedState &>(obj));
      return true;
    case tonNode_notFoundState::ID:
      func(static_cast<tonNode_notFoundState &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(tonNode_PreparedState &obj, const T &func) {
switch (obj.get_id()) {    case tonNode_preparedState::ID:
      func(create_tl_object<tonNode_preparedState>());
      return true;
    case tonNode_notFoundState::ID:
      func(create_tl_object<tonNode_notFoundState>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(validator_Group &obj, const T &func) {
  switch (obj.get_id()) {
    case validator_group::ID:
      func(static_cast<validator_group &>(obj));
      return true;
    case validator_groupEx::ID:
      func(static_cast<validator_groupEx &>(obj));
      return true;
    case validator_groupNew::ID:
      func(static_cast<validator_groupNew &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(validator_Group &obj, const T &func) {
switch (obj.get_id()) {    case validator_group::ID:
      func(create_tl_object<validator_group>());
      return true;
    case validator_groupEx::ID:
      func(create_tl_object<validator_groupEx>());
      return true;
    case validator_groupNew::ID:
      func(create_tl_object<validator_groupNew>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(validator_config_Local &obj, const T &func) {
  switch (obj.get_id()) {
    case validator_config_local::ID:
      func(static_cast<validator_config_local &>(obj));
      return true;
    case validator_config_random_local::ID:
      func(static_cast<validator_config_random_local &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(validator_config_Local &obj, const T &func) {
switch (obj.get_id()) {    case validator_config_local::ID:
      func(create_tl_object<validator_config_local>());
      return true;
    case validator_config_random_local::ID:
      func(create_tl_object<validator_config_random_local>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(validatorSession_Config &obj, const T &func) {
  switch (obj.get_id()) {
    case validatorSession_config::ID:
      func(static_cast<validatorSession_config &>(obj));
      return true;
    case validatorSession_configNew::ID:
      func(static_cast<validatorSession_configNew &>(obj));
      return true;
    case validatorSession_configVersioned::ID:
      func(static_cast<validatorSession_configVersioned &>(obj));
      return true;
    case validatorSession_configVersionedV2::ID:
      func(static_cast<validatorSession_configVersionedV2 &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(validatorSession_Config &obj, const T &func) {
switch (obj.get_id()) {    case validatorSession_config::ID:
      func(create_tl_object<validatorSession_config>());
      return true;
    case validatorSession_configNew::ID:
      func(create_tl_object<validatorSession_configNew>());
      return true;
    case validatorSession_configVersioned::ID:
      func(create_tl_object<validatorSession_configVersioned>());
      return true;
    case validatorSession_configVersionedV2::ID:
      func(create_tl_object<validatorSession_configVersionedV2>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(validatorSession_Message &obj, const T &func) {
  switch (obj.get_id()) {
    case validatorSession_message_startSession::ID:
      func(static_cast<validatorSession_message_startSession &>(obj));
      return true;
    case validatorSession_message_finishSession::ID:
      func(static_cast<validatorSession_message_finishSession &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(validatorSession_Message &obj, const T &func) {
switch (obj.get_id()) {    case validatorSession_message_startSession::ID:
      func(create_tl_object<validatorSession_message_startSession>());
      return true;
    case validatorSession_message_finishSession::ID:
      func(create_tl_object<validatorSession_message_finishSession>());
      return true;
    default:
      return false;
  }
}

/**
 * Calls specified function object with the specified object downcasted to the most-derived type.
 * \param[in] obj Object to pass as an argument to the function object.
 * \param[in] func Function object to which the object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
 */
template <class T>
bool downcast_call(validatorSession_round_Message &obj, const T &func) {
  switch (obj.get_id()) {
    case validatorSession_message_submittedBlock::ID:
      func(static_cast<validatorSession_message_submittedBlock &>(obj));
      return true;
    case validatorSession_message_approvedBlock::ID:
      func(static_cast<validatorSession_message_approvedBlock &>(obj));
      return true;
    case validatorSession_message_rejectedBlock::ID:
      func(static_cast<validatorSession_message_rejectedBlock &>(obj));
      return true;
    case validatorSession_message_commit::ID:
      func(static_cast<validatorSession_message_commit &>(obj));
      return true;
    case validatorSession_message_vote::ID:
      func(static_cast<validatorSession_message_vote &>(obj));
      return true;
    case validatorSession_message_voteFor::ID:
      func(static_cast<validatorSession_message_voteFor &>(obj));
      return true;
    case validatorSession_message_precommit::ID:
      func(static_cast<validatorSession_message_precommit &>(obj));
      return true;
    case validatorSession_message_empty::ID:
      func(static_cast<validatorSession_message_empty &>(obj));
      return true;
    default:
      return false;
  }
}

/**
* Constructs tl_object_ptr with the object of the same type as the specified object, calls the specified function.
 * \param[in] obj Object to get the type from.
 * \param[in] func Function object to which the new object will be passed.
 * \returns whether function object call has happened. Should always return true for correct parameters.
*/template <class T>
bool downcast_construct(validatorSession_round_Message &obj, const T &func) {
switch (obj.get_id()) {    case validatorSession_message_submittedBlock::ID:
      func(create_tl_object<validatorSession_message_submittedBlock>());
      return true;
    case validatorSession_message_approvedBlock::ID:
      func(create_tl_object<validatorSession_message_approvedBlock>());
      return true;
    case validatorSession_message_rejectedBlock::ID:
      func(create_tl_object<validatorSession_message_rejectedBlock>());
      return true;
    case validatorSession_message_commit::ID:
      func(create_tl_object<validatorSession_message_commit>());
      return true;
    case validatorSession_message_vote::ID:
      func(create_tl_object<validatorSession_message_vote>());
      return true;
    case validatorSession_message_voteFor::ID:
      func(create_tl_object<validatorSession_message_voteFor>());
      return true;
    case validatorSession_message_precommit::ID:
      func(create_tl_object<validatorSession_message_precommit>());
      return true;
    case validatorSession_message_empty::ID:
      func(create_tl_object<validatorSession_message_empty>());
      return true;
    default:
      return false;
  }
}

}  // namespace ton_api
}  // namespace ton 
