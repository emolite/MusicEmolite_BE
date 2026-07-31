ALTER TABLE crm.messages ADD COLUMN reply_to_message_id BIGINT NULL;
ALTER TABLE crm.messages ADD COLUMN forwarded_from_message_id BIGINT NULL;

ALTER TABLE crm.messages
  ADD CONSTRAINT fk_messages_reply_to_message
  FOREIGN KEY (reply_to_message_id) REFERENCES crm.messages(id) ON DELETE SET NULL;

ALTER TABLE crm.messages
  ADD CONSTRAINT fk_messages_forwarded_from_message
  FOREIGN KEY (forwarded_from_message_id) REFERENCES crm.messages(id) ON DELETE SET NULL;
