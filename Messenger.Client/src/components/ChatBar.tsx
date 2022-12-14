import dayjs from "dayjs";
import React, { useEffect, useState } from "react";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { selectAuthorization } from "../app/slices/authorizationSlice";
import { selectChatList } from "../app/slices/chatListSlice";
import {
  getChatListAsync,
  getChatListBySearchAsync,
} from "../app/thunks/chatsThuck";
import DateService from "../services/messenger/DateService";
import { ReactComponent as ProfileSVG } from "../assets/svg/profile.svg";
import { currentChatSliceActions } from "../app/slices/currentChatSlice";
import {
  selectLayoutComponent,
  setShowBlackBackground,
  setShowChatCreater,
  setShowProfile,
} from "../app/slices/layoutComponentSlice";

export const ChatBar = () => {
  const authorizationState = useAppSelector(selectAuthorization);
  const chatListState = useAppSelector(selectChatList);
  const dispatch = useAppDispatch();

  const [searchChatsValue, setSearchChatsValue] = useState<string>("");

  const shortLastMessage = (text: string): string => {
    if (text.length > 15) return `${text.slice(0, 15)}...`;

    return text;
  };

  const openProfile = () => {
    dispatch(setShowProfile(true));
    dispatch(setShowBlackBackground(true));
  };

  const openChatCreater = () => {
    dispatch(setShowChatCreater(true));
    dispatch(setShowBlackBackground(true));
  };

  const onChangeSearchChats = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchChatsValue(e.currentTarget.value);

    if (e.currentTarget.value.trim() !== "") {
      return dispatch(getChatListBySearchAsync(e.currentTarget.value));
    }
    dispatch(getChatListAsync());
  };

  return (
    <div className="chatsbar">
      <div className="chatsbar__header">
        <input
          className="chatsbar__header__input"
          type="text"
          placeholder="Search"
          value={searchChatsValue}
          onChange={onChangeSearchChats}
        />
      </div>
      <div className="chatsbar__chatlist">
        {chatListState.data.map((item) => (
          <div
            className="chatsbar__chatlist-item"
            key={item.chat.id}
            onClick={() => {
              dispatch(currentChatSliceActions.setCurrentChatData(item));
            }}
          >
            <img
              className="chatsbar__chatlist-item__avatar"
              src={
                item.chat.avatarLink
                  ? item.chat.avatarLink
                  : "https://i.pinimg.com/564x/55/76/29/5576291b493260f343e96dabf11f41d4.jpg"
              }
              alt=""
            />
            <div className="chatsbar__chatlist-item__data">
              <p className="chatsbar__chatlist-item__data__chat-name">
                {item.chat.title ??
                  item.chat.members?.find(
                    (m) => m.id != authorizationState?.data?.id
                  )?.displayName}
              </p>
              <p className="chatsbar__chatlist-item__data__last-message">
                {item.chat.lastMessageId && item.chat.lastMessageText
                  ? `${
                      item.chat.lastMessageAuthorDisplayName
                    }: ${shortLastMessage(item.chat.lastMessageText || "")}`
                  : ""}
              </p>
              <p className="chatsbar__chatlist-item__data__last-message-date-time">
                {item.chat.lastMessageDateOfCreate
                  ? DateService.getTime(item.chat.lastMessageDateOfCreate)
                  : ""}
              </p>
            </div>
          </div>
        ))}
      </div>
      <div className="chatsbar__button-profile" onClick={openProfile}>
        <ProfileSVG className="chatsbar__button-profile__profile-svg" />
      </div>
      <div className="chatsbar__button-create-chat" onClick={openChatCreater}>
        <div className="chatsbar__button-create-chat__line1"></div>
        <div className="chatsbar__button-create-chat__line2"></div>
      </div>
    </div>
  );
};
